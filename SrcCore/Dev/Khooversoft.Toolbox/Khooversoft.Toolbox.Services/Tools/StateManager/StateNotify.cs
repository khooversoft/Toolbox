// Copyright (c) KhooverSoft. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

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
