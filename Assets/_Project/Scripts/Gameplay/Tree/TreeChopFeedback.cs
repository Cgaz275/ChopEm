using System.Collections.Generic;
using System;
using UnityEngine;

public class TreeChopFeedback : MonoBehaviour
{
    private readonly List<GameObject> activeFeedbacks = new List<GameObject>();

    [SerializeField] private float exitDistance = 180f;
    [SerializeField] private float exitHeight = 140f;
    [SerializeField] private float exitAngle = 18f;
    [SerializeField] private float exitDuration = 0.22f;

    public void ResetFeedback(Transform container)
    {
        if (container == null) return;

        foreach (Transform child in container)
        {
            LeanTween.cancel(child.gameObject);

            if (child.name == "ChoppedChunkFeedback")
            {
                Destroy(child.gameObject);
            }
        }

        activeFeedbacks.Clear();
    }

    public void AnimateChunkPositions(
        IReadOnlyList<TreeChunkUI> chunks,
        float baseY,
        float chunkHeight,
        Action onComplete = null)
    {
        for (int i = 0; i < chunks.Count; i++)
        {
            RectTransform rect = chunks[i].GetComponent<RectTransform>();
            if (rect == null) continue;

            LeanTween.cancel(rect.gameObject);

            Vector2 targetPosition = new Vector2(0f, baseY + i * chunkHeight);
            if (i == chunks.Count - 1)
            {
                rect.anchoredPosition = new Vector2(0f, baseY + chunks.Count * chunkHeight);
            }

            Vector2 startPosition = rect.anchoredPosition;
            LTDescr tween = LeanTween.value(rect.gameObject, startPosition, targetPosition, exitDuration)
                .setEaseOutQuad()
                .setIgnoreTimeScale(true)
                .setOnUpdate((Vector2 position) => rect.anchoredPosition = position);

            if (i == chunks.Count - 1 && onComplete != null)
            {
                tween.setOnComplete(onComplete);
            }
        }
    }

    public void PlayChunk(TreeChunkUI chunk, ChopSide side)
    {
        if (chunk == null) return;

        activeFeedbacks.RemoveAll(feedback => feedback == null);

        RectTransform source = chunk.GetComponent<RectTransform>();
        if (source == null) return;

        GameObject flyingChunk = Instantiate(chunk.gameObject, source.parent);
        flyingChunk.name = "ChoppedChunkFeedback";
        activeFeedbacks.Add(flyingChunk);

        TreeChunkUI flyingChunkView = flyingChunk.GetComponent<TreeChunkUI>();
        if (flyingChunkView != null)
        {
            flyingChunkView.SetChopHighlight(false);
        }

        RectTransform flyingRect = flyingChunk.GetComponent<RectTransform>();
        flyingRect.SetSiblingIndex(source.GetSiblingIndex() + 1);
        flyingRect.localPosition = source.localPosition;
        flyingRect.localRotation = source.localRotation;
        flyingRect.localScale = source.localScale;

        CanvasGroup canvasGroup = flyingChunk.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = flyingChunk.AddComponent<CanvasGroup>();
        }

        canvasGroup.alpha = 1f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        float direction = side == ChopSide.Left ? -1f : 1f;
        Vector3 exitPosition = flyingRect.localPosition + new Vector3(
            direction * exitDistance,
            exitHeight,
            0f
        );

        LeanTween.moveLocal(flyingChunk, exitPosition, exitDuration)
            .setEaseOutQuad()
            .setIgnoreTimeScale(true)
            .setOnComplete(() => DestroyFeedback(flyingChunk));

        LeanTween.rotateLocal(flyingChunk, new Vector3(0f, 0f, direction * exitAngle), exitDuration)
            .setEaseOutQuad()
            .setIgnoreTimeScale(true);

        LeanTween.alphaCanvas(canvasGroup, 0f, exitDuration)
            .setEaseInQuad()
            .setIgnoreTimeScale(true);
    }

    private void DestroyFeedback(GameObject feedback)
    {
        activeFeedbacks.Remove(feedback);
        if (feedback != null)
        {
            Destroy(feedback);
        }
    }
}
