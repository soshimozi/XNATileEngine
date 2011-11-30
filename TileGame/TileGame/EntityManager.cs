using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TileEngine;

namespace TileGame
{
    class EntityManager
    {
        private static EntityManager _instance;
        private Dictionary<int, IGameEntity> _entities = new Dictionary<int, IGameEntity>();

        private EntityManager()
        {
        }

        public static EntityManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new EntityManager();
                }

                return _instance;
            }
        }

        public void RegisterEntity(IGameEntity entity)
        {
            _entities.Add(entity.Id, entity);
        }

        public IGameEntity this[int id]
        {
            get { return _entities[id]; }
        }
    }
}
