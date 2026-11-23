using System;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class TitleCloudDrift : MonoBehaviour
{
    [Serializable]
    private struct CloudLayer
    {
        public RectTransform rect;
        public Vector2 distance;
        [Min(1f)] public float period;
        public float phase;
    }

    [SerializeField] private CloudLayer[] layers = Array.Empty<CloudLayer>();

    private Vector2[] startingPositions;
    private float elapsed;

    private void OnEnable()
    {
        elapsed = 0f;
        startingPositions = new Vector2[layers.Length];
        for (int index = 0; index < layers.Length; index++)
        {
            if (layers[index].rect != null)
            {
                startingPositions[index] = layers[index].rect.anchoredPosition;
            }
        }
    }

    private void LateUpdate()
    {
        elapsed += Time.unscaledDeltaTime;
        for (int index = 0; index < layers.Length; index++)
        {
            CloudLayer layer = layers[index];
            if (layer.rect == null || layer.period <= 0f)
            {
                continue;
            }

            float angle = (elapsed % layer.period) / layer.period * (2f * Mathf.PI) + layer.phase;
            float displacement = Mathf.Sin(angle) - Mathf.Sin(layer.phase);
            layer.rect.anchoredPosition = startingPositions[index] + layer.distance * displacement;
        }
    }

    private void OnDisable()
    {
        if (startingPositions == null)
        {
            return;
        }
        for (int index = 0; index < layers.Length; index++)
        {
            if (layers[index].rect != null)
            {
                layers[index].rect.anchoredPosition = startingPositions[index];
            }
        }
    }
}
