using System.Collections.Generic;

namespace Engine.Graphics
{
    public class DrawBatch
    {
        protected List<GameObject> _gameObjectsList;
        protected GameObject[] _gameObjects;


        public DrawBatch()
        {
            _gameObjectsList = new List<GameObject>();
            _gameObjects = new GameObject[0];
        }

        public void Add(GameObject gameObject)
        {
            gameObject.OnDispose += GameObjectDispose;
            _gameObjectsList.Add(gameObject);
            _gameObjects = _gameObjectsList.ToArray();
        }

        public void AddRange(GameObject[] gameObjects)
        {
            foreach (var gameObject in gameObjects)
            {
                Add(gameObject);
            }
        }

        public void Remove(GameObject gameObject)
        {
            _gameObjectsList.Remove(gameObject);
            _gameObjects = _gameObjectsList.ToArray();
        }

        public void Draw()
        {
            for (var i = 0; i < _gameObjects.Length; i++)
            {
                _gameObjects[i].Draw();
            }
        }

        void GameObjectDispose(object sender)
        {
            _gameObjectsList.Remove((GameObject)sender);
            _gameObjects = _gameObjectsList.ToArray();
        }

    }
}
