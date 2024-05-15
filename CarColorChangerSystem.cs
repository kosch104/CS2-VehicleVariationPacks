using System;
using Colossal.Entities;
using Colossal.Logging;
using Game;
using Game.Prefabs;
using Game.Rendering;
using Game.UI;
using Game.Vehicles;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using PersonalCar = Game.Vehicles.PersonalCar;

namespace CarColorChanger
{
    public partial class CarColorChangerSystem : GameSystemBase
    {
        private static ILog Logger;
        private EntityQuery query;
        private UIUpdateState uiUpdateState;
        protected override void OnCreate()
        {
            base.OnCreate();
            Enabled = true;
            Logger = Mod.log;

            EntityQueryDesc desc = new EntityQueryDesc
            {
                Any = [
                    ComponentType.ReadOnly<ParkedCar>(),
                    ComponentType.ReadOnly<PersonalCar>(),
                ]
            };
            query = GetEntityQuery(desc);
            uiUpdateState = UIUpdateState.Create(World, 1024);
        }


        protected override void OnUpdate()
        {
            if (query == null || !uiUpdateState.Advance())
                return;
            //Logger.Info("Hello");

            var entities = query.ToEntityArray(Allocator.Temp);
            foreach (var entity in entities)
            {
                if (EntityManager.TryGetComponent<PrefabRef>(entity, out var prefabRef))
                {
                    if (EntityManager.HasBuffer<SubMesh>(prefabRef))
                    {
                        var subMesh = EntityManager.GetBuffer<SubMesh>(prefabRef);
                        if (subMesh.IsEmpty)
                            continue;
                        if(subMesh[0].m_SubMesh != Entity.Null && EntityManager.HasBuffer<ColorVariation>(subMesh[0].m_SubMesh))
                        {
                            var colorVariations = EntityManager.GetBuffer<ColorVariation>(subMesh[0].m_SubMesh);
                            for (int i = 0; i < colorVariations.Length; i++)
                            {
                                if (i < colorVariations.Length / 2)
                                {
                                    var col = colorVariations[i];
                                    col.m_ColorSet = new ColorSet(Color.black);
                                    colorVariations[i] = col;
                                }

                            }

                            //colorVariations = new DynamicBuffer<ColorVariation>();
                            //colorVariations[0] = new ColorVariation() { m_ColorSet = new ColorSet(Color.red) };
                        }
                    }
                }
            }
        }
    }

}