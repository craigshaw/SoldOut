using System;
using SoldOutBusiness.Services.Notifiers;
using System.Timers;
using System.Collections.Generic;
using System.Text;

namespace SoldOutSearchMonkey.Services
{
    internal class ResultAggregator : IResultAggregator
    {
        private INotifier _notifier;
        private object _summaryLock = new object();
        private AggregatedResults _results;
        private Timer _pulse;
        private int _reportingPeriod;

        public ResultAggregator(INotifier notifier, int reportingPeriod)
        {
            if (notifier == null)
                throw new ArgumentNullException(nameof(notifier));

            if (reportingPeriod < 1)
                throw new ArgumentOutOfRangeException(nameof(reportingPeriod), "Reporting period must be at least 1 second");

            _reportingPeriod = reportingPeriod;
            _notifier = notifier;
            _pulse = CreateAggreagtorPulse();
        }

        private Timer CreateAggreagtorPulse()
        {
            var pulse = new Timer(_reportingPeriod * 60 * 1000);
            pulse.Elapsed += Pulse;
            return pulse;
        }

        private void Pulse(object sender, ElapsedEventArgs e)
        {
            lock (_summaryLock)
            {
                // Generate and send a notification summarising what we've got stored
                StringBuilder notification = new StringBuilder($"I've harvested {_results.TotalItemsHarvested} results from {_results.TotalSearchesRan} searches in the last {_reportingPeriod} minutes\n");

                foreach (var condition in _results.ConditionalTotals)
                {
                    notification.Append($" {condition.Key} - {condition.Value.Total} total, {condition.Value.Suspicious} suspicious\n");
                }

                _notifier.PostMessage(notification.ToString());

                // Then reset the aggregate object
                _results = new AggregatedResults();
            }
        }

        public void Add(SearchSummary searchSummary)
        {
            // Add the given results to our aggregate
            lock (_summaryLock)
            {
                _results.TotalSearchesRan++;
                _results.TotalItemsHarvested += searchSummary.TotalResults;

                foreach (var condition in searchSummary.Summary)
                {
                    if (!_results.ConditionalTotals.ContainsKey(condition.Condition))
                        _results.ConditionalTotals[condition.Condition] = new ConditionalSummary();

                    _results.ConditionalTotals[condition.Condition].Total += condition.Total;
                    _results.ConditionalTotals[condition.Condition].Suspicious += condition.Suspicious;
                }
            }
        }

        public void Start()
        {
            if(!_pulse.Enabled)
            {
                _results = new AggregatedResults();
                _pulse.Start();
            }
        }

        public void Stop()
        {
            if (_pulse.Enabled)
            {
                _pulse.Stop();
            }
        }
    }

    class AggregatedResults
    {
        public AggregatedResults()
        {
            ConditionalTotals = new Dictionary<string, ConditionalSummary>();
        }

        public int TotalSearchesRan { get; set; }
        public int TotalItemsHarvested { get; set; }
        public IDictionary<string, ConditionalSummary> ConditionalTotals { get; set; }
    }
}
