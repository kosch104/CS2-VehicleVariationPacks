using System;
using System.Collections.Generic;
using Colossal.Entities;
using Colossal.Logging;
using Colossal.Serialization.Entities;
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

        //private UIUpdateState uiUpdateState;
        private PrefabSystem prefabSystem;
        public static CarColorChangerSystem Instance { get; private set; }
        private VariationPack _currentVariationPack;

        protected override void OnCreate()
        {
            base.OnCreate();
            Instance = this;
            Enabled = true;
            Logger = Mod.log;

            EntityQueryDesc desc = new EntityQueryDesc
            {
                Any =
                [
                    ComponentType.ReadOnly<ParkedCar>(),
                    ComponentType.ReadOnly<PersonalCar>(),
                ]
            };
            query = GetEntityQuery(desc);
            //uiUpdateState = UIUpdateState.Create(World, 1024);
            prefabSystem = World.GetOrCreateSystemManaged<PrefabSystem>();
            if (_currentVariationPack == null)
            {
                _currentVariationPack = VariationPack.Rdm();
            }
        }

        protected override void OnGameLoadingComplete(Purpose purpose, GameMode mode)
        {
            base.OnGameLoadingComplete(purpose, mode);
            if (mode == GameMode.MainMenu)
            {
                //UpdatePrefabs();
                UpdateEntities();
            }
        }

        public static void UpdatePrefabsManually()
        {
            Instance.UpdateEntities();
        }

        private void UpdatePrefabs()
        {
            EntityQueryDesc desc = new EntityQueryDesc
            {
                Any =
                [
                    ComponentType.ReadOnly<PersonalCarData>(),
                ],
                All =
                [
                    ComponentType.ReadOnly<VehicleData>(),
                    ComponentType.ReadOnly<CarData>(),
                ]
            };
            query = GetEntityQuery(desc);


            var entities = query.ToEntityArray(Allocator.Temp);
            foreach (var entity in entities)
            {
                if (prefabSystem.TryGetPrefab(entity, out PrefabBase prefabBase))
                {
                    if (prefabBase is CarPrefab prefab)
                    {
                        if (prefab.m_Meshes.Length == 0)
                            continue;
                        var mesh = prefab.m_Meshes[0].m_Mesh;
                        var colorProperties = mesh.GetComponent<ColorProperties>();
                        colorProperties.m_ColorVariations =
                        [
                            new ColorProperties.VariationSet()
                            {
                                m_Colors = [Color.red, Color.red, Color.red]
                            }
                        ];
                        prefabSystem.UpdatePrefab(prefab);
                    }
                }
            }
        }

        private void UpdateEntities()
        {
            EntityQueryDesc desc = new EntityQueryDesc
            {
                Any =
                [
                    ComponentType.ReadOnly<PersonalCarData>(),
                ]
            };
            query = GetEntityQuery(desc);
            var entities = query.ToEntityArray(Allocator.Temp);
            foreach (var entity in entities)
            {
                if (EntityManager.HasBuffer<SubMesh>(entity))
                {
                    var subMesh = EntityManager.GetBuffer<SubMesh>(entity);
                    if (subMesh.IsEmpty)
                        continue;
                    if (subMesh[0].m_SubMesh != Entity.Null && EntityManager.HasBuffer<ColorVariation>(subMesh[0].m_SubMesh))
                    {
                        var colorVariations = EntityManager.GetBuffer<ColorVariation>(subMesh[0].m_SubMesh);
                        //VariationPack.SaveDefault(colorVariations);
                        /*colorVariations.Clear();
                        colorVariations.Add(new ColorVariation()
                        {
                            m_ColorSet = new ColorSet(Color.black)
                        });*/

                        _currentVariationPack.FillColorVariations(ref colorVariations);

                    }
                }
            }
        }

        protected override void OnUpdate()
        {

        }
    }

}