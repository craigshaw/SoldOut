namespace SoldOutBusiness.Utilities.Conditions
{
    public interface IConditionResolver
    {
        int ConditionIdFromEBayConditionId(int eBayConditionId);
        string ConditionDescriptionFromConditionId(int conditionId);
    }
}
