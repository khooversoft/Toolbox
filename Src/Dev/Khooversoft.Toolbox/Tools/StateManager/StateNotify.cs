namespace Khooversoft.Toolbox
{
    public class StateNotify
    {
        public enum ActionTypes
        {
            Test,
            Set
        }

        public StateNotify(IStateItem item, ActionTypes actionType, bool result)
        {
            Verify.IsNotNull(nameof(item), item);

            Item = item;
            ActionType = actionType;
            Result = result;
        }

        public IStateItem Item { get; }

        public ActionTypes ActionType { get; }

        public bool Result { get; }
    }
}
