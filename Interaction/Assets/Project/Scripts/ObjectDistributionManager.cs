using System.Collections.Generic;
using UnityEngine;

namespace Project
{
    public class ObjectDistributionManager : MonoBehaviour
    {
        [System.Serializable]
        private struct Area {
            public Vector3 position;
            public Vector3 areaSize;
            public float spacing;
            public int seed;
        }

        [Header("Prefab and Area Settings")]
        [SerializeField] private List<GameObject> objectPrefabs = new List<GameObject>(); 
        [SerializeField] private List<Area> areas = new List<Area>();

        [Header("Perlin Noise Settings (Clustering)")]
        [SerializeField] private float perlinScale = 10f; 
        [Range(0f, 1f)] [SerializeField] private float clusterThreshold = 0.5f;

        [Header("Standard Noise Settings (Density)")]
        [Range(0f, 1f)] [SerializeField] private float densityThreshold = 0.5f;

        [Header("Falloff Settings")]
        [Range(1, 100)] [SerializeField] private float falloffSteepness = 35f;
        [Range(0, 1)] [SerializeField] private float falloffPosition = .03f;

        [Header("Gizmo Settings")]
        [SerializeField] private float gizmoSize = 0.2f;
        [SerializeField] private bool visualizeBoundingBox = false; 
        [SerializeField] private bool visualizeDensity = false; 
        [SerializeField] private bool visualizeEmpty = false; 

        private Vector3 noiseOffset;

        void Start() {
            foreach (Area area in areas) {
                InitializeSeed(area.seed);
                GenerateObjects(area.position, area.areaSize, area.spacing);
            }
        }

        void InitializeSeed(int seed) {
            Random.InitState(seed);
            noiseOffset = new Vector3(Random.value * 10000f, Random.value * 10000f, Random.value * 10000f);
        }

        private void GenerateObjects(Vector3 center, Vector3 areaSize, float spacing) {
            for (float x = -areaSize.x / 2f; x <= areaSize.x / 2f; x += spacing) {
                for (float y = -areaSize.y / 2f; y <= areaSize.y / 2f; y += spacing) {
                    for (float z = -areaSize.z / 2f; z <= areaSize.z / 2f; z += spacing) {
                        Vector3 position = new Vector3(x, y, z);

                        float perlinValue = ComputePerlinNoise(position);
                        float densityValue = Random.Range(0f, 1f);
                        if (ShouldPlaceObject(perlinValue, densityValue)) 
                            Instantiate(
                                objectPrefabs[Random.Range(0, objectPrefabs.Count - 1)], 
                                position + center + new Vector3(Random.Range(-spacing/2f, spacing/2f), Random.Range(-spacing/2f, spacing/2f), Random.Range(-spacing/2f, spacing/2f)), 
                                Quaternion.Euler(new Vector3(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360)))
                            );
                    }
                }
            }
        }

        private bool ShouldPlaceObject(float perlinValue, float densityValue) => perlinValue > clusterThreshold && densityValue > densityThreshold;

        private float ComputePerlinNoise(Vector3 position) {
            float xScaled = (position.x + noiseOffset.x) / perlinScale; 
            float yScaled = (position.y + noiseOffset.y) / perlinScale; 
            float zScaled = (position.z + noiseOffset.z) / perlinScale;  

            float xy = Mathf.PerlinNoise(xScaled, yScaled);
            float yz = Mathf.PerlinNoise(yScaled, zScaled);
            float xz = Mathf.PerlinNoise(xScaled, zScaled);

            float yx = Mathf.PerlinNoise(yScaled, xScaled);
            float zy = Mathf.PerlinNoise(zScaled, yScaled);
            float zx = Mathf.PerlinNoise(zScaled, xScaled);

            return (xy + yz + xz + yx + zy + zx) / 6f;
        }

        private float ComputeFalloffModifier(Vector3 position, Vector3 areaSize) {
            float minDistance = float.MaxValue;

            for (int i = 0; i < 3; i++) {
                float distance = Mathf.Min(Mathf.Abs(-areaSize[i] / 2 - position[i]), Mathf.Abs(+areaSize[i] / 2 - position[i])) / areaSize[i] / 2;
                minDistance = minDistance > distance ? distance : minDistance;
            }

            float result = 1f / (1f + Mathf.Exp(-falloffSteepness * (minDistance - falloffPosition)));
            return result;
        }

        #if UNITY_EDITOR
        private void OnDrawGizmosSelected() {
            foreach (Area area in areas) {
                DrawNoise(area.seed, area.areaSize, area.position, area.spacing);
                DrawBoundingBox(area.position, area.areaSize);
            }
        }

        private void DrawNoise(int seed, Vector3 areaSize, Vector3 center, float spacing) {
            if (!visualizeDensity) return;

            InitializeSeed(seed);

            for (float x = -areaSize.x / 2f; x <= areaSize.x / 2f; x += spacing) {
                for (float y = -areaSize.y / 2f; y <= areaSize.y / 2f; y += spacing) {
                    for (float z = -areaSize.z / 2f; z <= areaSize.z / 2f; z += spacing) {
                        Vector3 position = new Vector3(x, y, z);

                        float perlinValue = ComputePerlinNoise(position);
                        float densityValue = Random.Range(0f, 1f);
                        float falloffModifier = ComputeFalloffModifier(position, areaSize);

                        Color color = DetermineGizmoColor(perlinValue * falloffModifier, densityValue);
                        DrawGizmo(position + center + new Vector3(Random.Range(-spacing/2f, spacing/2f), Random.Range(-spacing/2f, spacing/2f), Random.Range(-spacing/2f, spacing/2f)), color);
                    }
                }
            }
        }

        private Color DetermineGizmoColor(float perlinValue, float densityValue) {
            if (perlinValue > clusterThreshold) return densityValue > densityThreshold ? Color.green : Color.yellow;
            else return Color.gray;
        }

        private void DrawGizmo(Vector3 position, Color color) {
            if (!visualizeEmpty && (color == Color.grey || color == Color.yellow)) return;

            Gizmos.color = color;
            Gizmos.DrawCube(position + transform.position, Vector3.one * gizmoSize);
        }

        private void DrawBoundingBox(Vector3 center, Vector3 areaSize) {
            if (!visualizeBoundingBox) return;

            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(center, areaSize);
        }
        #endif
    }
}
