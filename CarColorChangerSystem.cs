using System;
using System.Collections.Generic;
using Colossal.Entities;
using Colossal.Logging;
using Colossal.Serialization.Entities;
using Game;
using Game.Common;
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
                    ComponentType.ReadOnly<PersonalCarData>(),
                ]
            };
            query = GetEntityQuery(desc);
            //uiUpdateState = UIUpdateState.Create(World, 1024);
            prefabSystem = World.GetOrCreateSystemManaged<PrefabSystem>();
            SaveDefaultVariations();
            if (_currentVariationPack == null)
            {
                _currentVariationPack = VariationPack.Test();
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

        public static void UpdateEntitiesManually()
        {
            Instance.UpdateEntities();
        }

        private void SaveDefaultVariations()
        {
            var entities = query.ToEntityArray(Allocator.Temp);
            VariationPack pack = new VariationPack();
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
                        var prefabName = prefabSystem.GetPrefabName(entity);
                        pack.SavePrefabVariations(prefabName, colorVariations);
                    }
                }
            }
            pack.Name = "Vanilla";
            pack.Save();
        }

        private void UpdateEntities()
        {
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
                        var prefabName = prefabSystem.GetPrefabName(entity);
                        _currentVariationPack.FillColorVariations(prefabName, ref colorVariations);
                    }
                }
            }
        }

        protected override void OnUpdate()
        {

        }

        public static void LoadVariationPack(string value)
        {
            if (value.StartsWith("debug_"))
            {
                value = value.Replace("debug_", "");
                if (value == "Test")
                {
                    Instance._currentVariationPack = VariationPack.Test();
                    UpdateEntitiesManually();
                }
                if (value == "CrazyColors")
                {
                    Instance._currentVariationPack = VariationPack.Rdm();
                    UpdateEntitiesManually();
                }
                if (value == "Default")
                {
                    Instance._currentVariationPack = VariationPack.Default();
                    UpdateEntitiesManually();
                }

                return;
            }
            Instance._currentVariationPack = VariationPack.Load(value);
            UpdateEntitiesManually();
        }
    }

}