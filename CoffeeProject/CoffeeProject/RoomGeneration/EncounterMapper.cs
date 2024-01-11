using CoffeeProject.Encounters;
using MagicDustLibrary.Logic;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeProject.RoomGeneration
{
    public class EncounterMapper
    {
        private readonly Dictionary<string, Encounter> _encounters = [];

        public void AddEncounter(string name, Encounter encounter)
        {
            _encounters.Add(name, encounter);
        }

        public void AddEncounter(Encounter encounter)
        {
            AddEncounter(encounter.GetType().Name, encounter);
        }

        public void AddEncounter<T>() where T : Encounter, new()
        {
            AddEncounter(new T());
        }

        public void InvokeAll(IControllerProvider state, GraphInfo graphInfo, Room[] rooms, Func<Point, Vector2> positionTranslator)
        {
            foreach (var room in graphInfo.Positions)
            {
                var roomNode = graphInfo.Graph.Rooms.Where(it => it.RoomNumber == room.Key).First();
                foreach (var encounter in roomNode.RoomInfo.Encounters)
                {
                    if (_encounters.TryGetValue(encounter.Name, out Encounter value))
                    {
                        value.Invoke(state, positionTranslator(encounter.Position + room.Value.Location), rooms[room.Key]);
                    }
                }
            }
        }
    }
}
