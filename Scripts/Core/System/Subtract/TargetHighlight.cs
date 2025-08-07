using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameJam_HIKU
{

    /// <summary>�����\�I�u�W�F�N�g�̃n�C���C�g</summary>
    public class TargetHighlight : MonoBehaviour
    {
        [field: SerializeField] public GameObject HighlightEffect { get; private set; }
        [field: SerializeField] public SpriteRenderer HighlightSprite { get; private set; }
        [field: SerializeField] public Color HighlightColor { get; private set; } = Color.yellow;
        [field: SerializeField] public Color HoverColor { get; private set; } = Color.red;
        [field: SerializeField] public float PulseSpeed { get; private set; } = 2f;

        private List<RemovableObject> highlightedTargets = new List<RemovableObject>();
        private RemovableObject hoverTarget = null;
        private Dictionary<RemovableObject, GameObject> highlightObjects = new Dictionary<RemovableObject, GameObject>();

        /// <summary>�^�[�Q�b�g���n�C���C�g�\��</summary>
        public void HighlightTargets(IReadOnlyList<RemovableObject> targets)
        {
            // �����n�C���C�g���N���A
            ClearHighlights();

            // �V�����^�[�Q�b�g���n�C���C�g
            foreach (var target in targets)
            {
                AddHighlight(target, HighlightColor);
            }

            highlightedTargets.Clear();
            highlightedTargets.AddRange(targets);
        }

        /// <summary>�z�o�[�^�[�Q�b�g�ݒ�</summary>
        public void SetHoverTarget(RemovableObject target)
        {
            // �O�̃z�o�[��ʏ�n�C���C�g�ɖ߂�
            if (hoverTarget != null && highlightObjects.ContainsKey(hoverTarget))
            {
                UpdateHighlightColor(hoverTarget, HighlightColor);
            }

            hoverTarget = target;

            // �V�����z�o�[�������\��
            if (hoverTarget != null && highlightObjects.ContainsKey(hoverTarget))
            {
                UpdateHighlightColor(hoverTarget, HoverColor);
            }
        }

        /// <summary>�S�n�C���C�g���N���A</summary>
        public void ClearHighlights()
        {
            foreach (var kvp in highlightObjects)
            {
                if (kvp.Value != null)
                {
                    Destroy(kvp.Value);
                }
            }

            highlightObjects.Clear();
            highlightedTargets.Clear();
            hoverTarget = null;
        }

        /// <summary>�ʃn�C���C�g�ǉ�</summary>
        private void AddHighlight(RemovableObject target, Color color)
        {
            if (target == null || highlightObjects.ContainsKey(target)) return;

            GameObject highlight = null;

            if (HighlightEffect != null)
            {
                highlight = Instantiate(HighlightEffect, target.transform);
            }
            else if (HighlightSprite != null)
            {
                highlight = new GameObject($"Highlight_{target.name}");
                highlight.transform.SetParent(target.transform);
                highlight.transform.localPosition = Vector3.zero;

                var spriteRenderer = highlight.AddComponent<SpriteRenderer>();
                spriteRenderer.sprite = HighlightSprite.sprite;
                spriteRenderer.color = color;
                spriteRenderer.sortingOrder = 100;
            }

            if (highlight != null)
            {
                highlightObjects[target] = highlight;

                // �p���X����
                var pulseEffect = highlight.AddComponent<PulseEffect>();
                pulseEffect.Initialize(PulseSpeed);
            }
        }

        /// <summary>�n�C���C�g�F�X�V</summary>
        private void UpdateHighlightColor(RemovableObject target, Color color)
        {
            if (!highlightObjects.TryGetValue(target, out var highlight) || highlight == null) return;

            var spriteRenderer = highlight.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.color = color;
            }
        }
    }

}