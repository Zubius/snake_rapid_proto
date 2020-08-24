using System.Collections.Generic;
using System.Linq;
using ECS;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

namespace View
{
    public class GameBoardView : MonoBehaviour
    {
        private List<ChangeableObject> _changeableObjects;
        private EntityManager _entityManager;

        private bool _inited = false;

        internal void InitView(GameObject protoCell, int x, int y, float dist)
        {
            _changeableObjects = new List<ChangeableObject>(x * y);

            var settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, null);
            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            var entity = GameObjectConversionUtility.ConvertGameObjectHierarchy(protoCell, settings);

            //center elements infront of camera
            float offset = -1 * x / 2f + 0.5f;

            for (int i = 0; i < x; i++)
            {
                for (int j = 0; j < y; j++)
                {
                    var entityInstance = _entityManager.Instantiate(entity);
                    _entityManager.SetComponentData(entityInstance, new Translation {Value = new Vector3(GetPos(i),  0, GetPos(j))});

                    _changeableObjects.Add(new ChangeableObject
                    {
                        Entity = entityInstance,
                        Color = Color.white
                    });
                }
            }

            _inited = true;

            float GetPos(int i)
            {
                return (i + offset) * dist;
            }
        }

        internal void SetBoardState(GameCell[] Field)
        {
            if (!_inited)
            {
                Debug.LogError($"View has not been inited!");
                return;
            }

            if (Field.Length != _changeableObjects.Count)
            {
                Debug.LogError($"Different dimensions!");
                return;
            }

            var cells = new NativeArray<GameCell>(_changeableObjects.Count, Allocator.TempJob);
            var indexes = new NativeArray<int>(_changeableObjects.Count, Allocator.TempJob);
            var colors = new NativeArray<Color>(_changeableObjects.Count, Allocator.TempJob);

            for (int i = 0; i < cells.Length; i++)
            {
                cells[i] = Field[i];
                indexes[i] = i;
                colors[i] = _changeableObjects[i].Color;
            }

            var job = new CellChanger()
            {
                Cells = cells,
                Indexes = indexes,
                Colors = colors,
            };

            var jobHandler = job.Schedule(_changeableObjects.Count, 10);
            jobHandler.Complete();

            for (int i = 0; i < _changeableObjects.Count; i++)
            {
                var renderMesh = _entityManager.GetSharedComponentData<RenderMesh>(_changeableObjects[i].Entity);
                var mat = new UnityEngine.Material(renderMesh.material);
                mat.SetColor("_Color", colors[i]);
                renderMesh.material = mat;

                _entityManager.SetSharedComponentData(_changeableObjects[i].Entity, renderMesh);
            }

            cells.Dispose();
            colors.Dispose();
            indexes.Dispose();
        }
    }
}
