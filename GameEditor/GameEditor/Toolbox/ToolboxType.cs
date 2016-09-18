using System;
using Engine.Mechanics;
using GameLibrary;
using GameLibrary.Objects;
using TheGoo;

namespace GameEditor.ToolBox
{
    [Serializable]
    public class ToolboxType
    {

        protected readonly GameObjectType _objectTypeName;
        protected readonly ResourceIdentifier _resourceIdentifier;
        protected readonly int _subObjectType;

        public ToolboxType(GameObjectType objectTypeName, ResourceIdentifier resource)
        {
            _objectTypeName = objectTypeName;
            _resourceIdentifier = resource;
            _subObjectType = 0;
        }

        public ToolboxType(GameObjectType objectTypeName, ResourceIdentifier resource, int subObjectType)
        {
            _objectTypeName = objectTypeName;
            _resourceIdentifier = resource;
            _subObjectType = subObjectType;
        }

        public GameObjectType ObjectTypeName
        {
            get { return _objectTypeName; }
        }

        public ResourceIdentifier ResourceIdentifier
        {
            get { return _resourceIdentifier; }
        }

        public int SubObjectType
        {
            get { return _subObjectType; }
        }

    }
}
