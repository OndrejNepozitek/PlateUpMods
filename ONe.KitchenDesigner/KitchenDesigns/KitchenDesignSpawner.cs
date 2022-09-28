// using System;
// using System.Collections.Generic;
// using System.Linq;
// using Kitchen;
// using Kitchen.Layouts;
// using Kitchen.Layouts.Features;
// using KitchenData;
// using Unity.Entities;
// using UnityEngine;
// using Random = UnityEngine.Random;
// using Seed = Kitchen.Seed;
//
// namespace ONe.KitchenDesigner.KitchenDesigns;
//
// public class KitchenDesignSpawner : MonoBehaviour
// {
//     private static SeededLayout _lastGeneratedLayout;
//
//     private void Update()
//     {
//         if (Input.GetKeyDown(KeyCode.F3) && Input.GetKey(KeyCode.LeftControl))
//         {
//             Debug.Log("CTRL + F3: KitchenDesigner");
//             Run();
//         }
//     }
//
//     private void Run()
//     {
//         var encoded = Plugin.ConfigKitchenDesignerLayout.Value;
//
//         if (string.IsNullOrEmpty(encoded))
//         {
//             throw new InvalidOperationException("Kitchen design not set");
//         }
//
//         var kitchenDesign = KitchenDesignLoader.Load(encoded);
//
//         var random = new System.Random();
//         var seed = "design" + random.Next(10, 100);
//         var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
//         var seedObject = new Seed(seed);
//         
//         CenterLayout(kitchenDesign.Blueprint);
//
//         Random.State state = Random.state;
//         Random.InitState(seedObject.IntValue);
//         var layoutEntity = ConstructLayout(
//             entityManager,
//             kitchenDesign.Blueprint,
//             kitchenDesign.Profile,
//             kitchenDesign.Setting,
//             seedObject.IntValue);
//         var mapItem = CreateMapItem(layoutEntity, kitchenDesign.Setting, seedObject);
//         Random.state = state;
//
//         var seededLayout = new SeededLayout(seed, kitchenDesign.Blueprint, layoutEntity, mapItem);
//         ShowOnPedestal(seededLayout);
//     }
//     
//     private static void ShowOnPedestal(SeededLayout layout)
//     {
//         var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
//         
//         if (_lastGeneratedLayout != null)
//         {
//             entityManager.DestroyEntity(_lastGeneratedLayout.MapItem);
//             entityManager.DestroyEntity(_lastGeneratedLayout.LayoutEntity);
//         }
//         
//         var selectedLayoutPedestal = entityManager.CreateEntityQuery(typeof(SSelectedLayoutPedestal)).GetSingletonEntity();
//                 
//         entityManager.AddComponentData<CItemHolder>(selectedLayoutPedestal, (CItemHolder) layout.MapItem);
//         entityManager.AddComponentData<CHeldBy>(layout.MapItem, (CHeldBy) selectedLayoutPedestal);
//
//         _lastGeneratedLayout = layout;
//     }
//     
//     private static Entity ConstructLayout(
//         EntityManager em,
//         LayoutBlueprint layoutBlueprintFixed,
//         LayoutProfile profile,
//         RestaurantSetting setting,
//         int seed)
//     {
//         var entity = em.CreateEntity((ComponentType)typeof(CStartingItem), (ComponentType)typeof(CLayoutRoomTile),
//             (ComponentType)typeof(CLayoutFeature), (ComponentType)typeof(CLayoutAppliancePlacement));
//         var layoutDecorator = (LayoutDecorator)null;
//         var layoutBlueprint = (LayoutBlueprint)null;
//         var flag = false;
//         for (int index = 0; index < CreateLayoutHelper.MapAttemptsMax; ++index)
//         {
//             try
//             {
//                 layoutBlueprint = layoutBlueprintFixed;
//                 layoutDecorator = new LayoutDecorator(layoutBlueprint, profile, setting);
//                 layoutDecorator.AttemptDecoration();
//                 flag = true;
//                 break;
//             }
//             catch (LayoutFailureException ex)
//             {
//             }
//         }
//
//         if (!flag || layoutDecorator?.Decorations == null)
//         {
//             Console.WriteLine((object)string.Format("Giving up after {0} attempts",
//                 (object)CreateLayoutHelper.MapAttemptsMax));
//             em.DestroyEntity(entity);
//             return new Entity();
//         }
//
//         layoutBlueprint.ToEntity(em, entity);
//         DynamicBuffer<CLayoutAppliancePlacement> buffer = em.GetBuffer<CLayoutAppliancePlacement>(entity);
//         foreach (CLayoutAppliancePlacement decoration in layoutDecorator.Decorations)
//             buffer.Add(decoration);
//         
//         return entity;
//     }
//     
//     /// <summary>
//     /// Creates a map item entity from a given layout, setting and seed.
//     /// 
//     /// This is the same function as found in:
//     /// <see cref="Kitchen.CreateSeededRuns" />
//     /// <see cref="Kitchen.HandleLayoutRequests" />
//     /// <see cref="Kitchen.SetSeededRunOverride" />
//     ///
//     /// No modifications were needed. Could be also called via reflection.
//     /// </summary>
//     private static Entity CreateMapItem(Entity layout, RestaurantSetting setting, Seed seed)
//     {
//         var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
//         Entity entity = entityManager.CreateEntity((ComponentType)typeof(CCreateItem), (ComponentType)typeof(CHeldBy));
//         entityManager.SetComponentData<CCreateItem>(entity, new CCreateItem()
//         {
//             ID = AssetReference.MapItem
//         });
//         entityManager.AddComponentData<CItemLayoutMap>(entity, new CItemLayoutMap()
//         {
//             Layout = layout
//         });
//         entityManager.AddComponentData<CSetting>(entity, new CSetting()
//         {
//             RestaurantSetting = setting.ID,
//             FixedSeed = seed
//         });
//         return entity;
//     }
//     
//     /// <summary>
//     /// Re-centers a given layout blueprint.
//     /// 
//     /// This is the same function as found in:
//     /// <see cref="Kitchen.Layouts.Modules.RecentreLayout" />
//     ///
//     /// No modifications were needed. Could be also called via reflection.
//     /// </summary>
//     private void CenterLayout(LayoutBlueprint blueprint)
//     {
//         if (blueprint.Tiles.Count == 0)
//             return;
//         
//         Dictionary<LayoutPosition, Room> tiles = blueprint.Tiles;
//         Vector2 min = new Vector2((float) tiles.Select<KeyValuePair<LayoutPosition, Room>, int>((Func<KeyValuePair<LayoutPosition, Room>, int>) (r => r.Key.x)).Min(), (float) tiles.Select<KeyValuePair<LayoutPosition, Room>, int>((Func<KeyValuePair<LayoutPosition, Room>, int>) (r => r.Key.y)).Min());
//         Vector2 new_min = -(new Vector2((float) tiles.Select<KeyValuePair<LayoutPosition, Room>, int>((Func<KeyValuePair<LayoutPosition, Room>, int>) (r => r.Key.x)).Max(), (float) tiles.Select<KeyValuePair<LayoutPosition, Room>, int>((Func<KeyValuePair<LayoutPosition, Room>, int>) (r => r.Key.y)).Max()) - min) / 2f;
//         new_min = new Vector2((float) Mathf.FloorToInt(new_min.x), (float) Mathf.FloorToInt(new_min.y));
//         blueprint.Tiles = tiles.ToDictionary<KeyValuePair<LayoutPosition, Room>, LayoutPosition, Room>((Func<KeyValuePair<LayoutPosition, Room>, LayoutPosition>) (r => translate((Vector2) r.Key)), (Func<KeyValuePair<LayoutPosition, Room>, Room>) (r => r.Value));
//         blueprint.Features = blueprint.Features.Select<Feature, Feature>((Func<Feature, Feature>) (f => new Feature(translate((Vector2) f.Tile1), translate((Vector2) f.Tile2), f.Type))).ToList<Feature>();
//
//         LayoutPosition translate(Vector2 input) => (LayoutPosition) (input - min + new_min);
//     }
// }