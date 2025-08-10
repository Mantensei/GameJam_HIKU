using MantenseiLib;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameJam_HIKU
{
    /// <summary>削除可能オブジェクトのハイライト表示</summary>
    public class TargetHighlight : MonoBehaviour
    {
        [field: SerializeField] public Color HighlightColor { get; private set; } = new Color(1f, 1f, 0f, 0.5f);
        [field: SerializeField] public Color HoverColor { get; private set; } = new Color(1f, 0f, 0f, 0.7f);
        [field: SerializeField] public bool UseOutline { get; private set; } = false;
        [field: SerializeField] public Material OutlineMaterial { get; private set; }

        private Dictionary<RemovableObject, HighlightState> highlightStates = new Dictionary<RemovableObject, HighlightState>();
        private RemovableObject currentHoverTarget;

        private class HighlightState
        {
            public Color originalColor;
            public Material originalMaterial;
            public SpriteRenderer spriteRenderer;
            public Renderer targetRenderer;
        }

        /// <summary>削除可能ターゲットをハイライト</summary>
        public void HighlightTargets(IReadOnlyList<RemovableObject> targets)
        {
            // 既存のハイライトをクリア（リストにないもの）
            var toRemove = new List<RemovableObject>();
            foreach (var kvp in highlightStates)
            {
                if (!targets.Contains(kvp.Key))
                {
                    RestoreOriginal(kvp.Value);
                    toRemove.Add(kvp.Key);
                }
            }
            foreach (var key in toRemove)
            {
                highlightStates.Remove(key);
            }

            // 新規ターゲットをハイライト
            foreach (var target in targets)
            {
                if (target == null) continue;

                if (!highlightStates.ContainsKey(target))
                {
                    ApplyHighlight(target, false);
                }
            }
        }

        /// <summary>ホバー中のターゲットを設定</summary>
        public void SetHoverTarget(RemovableObject target)
        {
            // 前のホバーターゲットを通常ハイライトに戻す
            if (currentHoverTarget != null && currentHoverTarget != target)
            {
                if (highlightStates.ContainsKey(currentHoverTarget))
                {
                    ApplyHighlight(currentHoverTarget, false);
                }
            }

            // 新しいホバーターゲットを特別ハイライト
            currentHoverTarget = target;
            if (currentHoverTarget != null && highlightStates.ContainsKey(currentHoverTarget))
            {
                ApplyHighlight(currentHoverTarget, true);
            }
        }

        /// <summary>ハイライトを適用</summary>
        private void ApplyHighlight(RemovableObject target, bool isHover)
        {
            var renderer = target.GetComponent<SpriteRenderer>();
            if (renderer == null)
            {
                renderer = target.GetComponentInChildren<SpriteRenderer>();
            }

            if (renderer == null) return;

            // 初回の場合、元の状態を保存
            if (!highlightStates.ContainsKey(target))
            {
                var state = new HighlightState
                {
                    originalColor = renderer.color,
                    originalMaterial = renderer.material,
                    spriteRenderer = renderer,
                    targetRenderer = renderer
                };
                highlightStates[target] = state;
            }

            var highlightState = highlightStates[target];

            // 色変更
            Color targetColor = isHover ? HoverColor : HighlightColor;
            renderer.color = Color.Lerp(highlightState.originalColor, targetColor, 0.5f);

            // アウトライン適用（オプション）
            if (UseOutline && OutlineMaterial != null)
            {
                renderer.material = OutlineMaterial;
            }
        }

        /// <summary>元の状態に戻す</summary>
        private void RestoreOriginal(HighlightState state)
        {
            if (state.spriteRenderer != null)
            {
                state.spriteRenderer.color = state.originalColor;
                state.spriteRenderer.material = state.originalMaterial;
            }
        }

        /// <summary>全てのハイライトをクリア</summary>
        public void ClearHighlights()
        {
            foreach (var kvp in highlightStates)
            {
                RestoreOriginal(kvp.Value);
            }
            highlightStates.Clear();
            currentHoverTarget = null;
        }

        void OnDestroy()
        {
            ClearHighlights();
        }
    }
}