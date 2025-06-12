using System.Collections.Generic;
using MCube;
using Unity.VisualScripting;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Ingame
{
    [ExecuteAlways]
    public class BSP3DTestbedModel : SerializedMonoBehaviour
    {
        public BSP3DModel testbedModel;
        public BSP3DModel playModel;

        [SerializeField]
        private Vector3Int size;
        public Vector3Int Size { get => size; set => SetSize(value); }

        public void OnEnable()
        {
            EnsureBSP3DModels();
        }

        private void SetSize(Vector3Int size)
        {
            testbedModel.mapAsset.size = size;
            playModel.mapAsset.size = size;
            this.size = size;
        }

        private void EnsureBSP3DModels()
        {
            EnsureModel(ref testbedModel, nameof(testbedModel));
            EnsureModel(ref playModel, nameof(playModel));
        }

        private void EnsureModel(ref BSP3DModel model, string name)
        {
            Transform existing = transform.Find(name);

            if (existing == null)
            {
                GameObject go = new(name);
                go.transform.SetParent(transform);
                model = go.AddComponent<BSP3DModel>();
            }
            else
            {
                model = existing.GetComponent<BSP3DModel>();
                if (model == null)
                    model = existing.gameObject.AddComponent<BSP3DModel>();
            }

            if (!model.GetComponent<ScalarFieldModel>())
                model.gameObject.AddComponent<ScalarFieldModel>();
        }
    }
}