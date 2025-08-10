using MantenseiLib;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameJam_HIKU
{
    /// <summary>�폜�\�I�u�W�F�N�g�̃n�C���C�g�\��</summary>
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

        /// <summary>�폜�\�^�[�Q�b�g���n�C���C�g</summary>
        public void HighlightTargets(IReadOnlyList<RemovableObject> targets)
        {
            // �����̃n�C���C�g���N���A�i���X�g�ɂȂ����́j
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

            // �V�K�^�[�Q�b�g���n�C���C�g
            foreach (var target in targets)
            {
                if (target == null) continue;

                if (!highlightStates.ContainsKey(target))
                {
                    ApplyHighlight(target, false);
                }
            }
        }

        /// <summary>�z�o�[���̃^�[�Q�b�g��ݒ�</summary>
        public void SetHoverTarget(RemovableObject target)
        {
            // �O�̃z�o�[�^�[�Q�b�g��ʏ�n�C���C�g�ɖ߂�
            if (currentHoverTarget != null && currentHoverTarget != target)
            {
                if (highlightStates.ContainsKey(currentHoverTarget))
                {
                    ApplyHighlight(currentHoverTarget, false);
                }
            }

            // �V�����z�o�[�^�[�Q�b�g����ʃn�C���C�g
            currentHoverTarget = target;
            if (currentHoverTarget != null && highlightStates.ContainsKey(currentHoverTarget))
            {
                ApplyHighlight(currentHoverTarget, true);
            }
        }

        /// <summary>�n�C���C�g��K�p</summary>
        private void ApplyHighlight(RemovableObject target, bool isHover)
        {
            var renderer = target.GetComponent<SpriteRenderer>();
            if (renderer == null)
            {
                renderer = target.GetComponentInChildren<SpriteRenderer>();
            }

            if (renderer == null) return;

            // ����̏ꍇ�A���̏�Ԃ�ۑ�
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

            // �F�ύX
            Color targetColor = isHover ? HoverColor : HighlightColor;
            renderer.color = Color.Lerp(highlightState.originalColor, targetColor, 0.5f);

            // �A�E�g���C���K�p�i�I�v�V�����j
            if (UseOutline && OutlineMaterial != null)
            {
                renderer.material = OutlineMaterial;
            }
        }

        /// <summary>���̏�Ԃɖ߂�</summary>
        private void RestoreOriginal(HighlightState state)
        {
            if (state.spriteRenderer != null)
            {
                state.spriteRenderer.color = state.originalColor;
                state.spriteRenderer.material = state.originalMaterial;
            }
        }

        /// <summary>�S�Ẵn�C���C�g���N���A</summary>
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