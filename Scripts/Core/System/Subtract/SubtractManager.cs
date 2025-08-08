using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MantenseiLib;
using System;

namespace GameJam_HIKU
{
    /// <summary>�u�Ђ��v�V�X�e���S�̂̓����Ǘ�</summary>
    public class SubtractManager : MonoBehaviour
    {
        [Header("Components")]
        [GetComponent] private SubtractSkill skill;
        [GetComponent] private SubtractTargetDetector targetDetector;
        [GetComponent] private SubtractCursor cursor;
        [GetComponent] private SubtractModeUI modeUI;
        [GetComponent] private TargetHighlight highlight;

        [field: SerializeField] public bool EnableClick { get; private set; } = true;

        public bool IsSubtractMode => skill?.IsSkillActive ?? false;
        public RemovableObject CurrentTarget => targetDetector?.CurrentHoverTarget;

        public event Action onRemoveSuccess;

        void Start()
        {
            InitializeComponents();
            SetupEventListeners();
        }

        void Update()
        {
            if (IsSubtractMode)
            {
                UpdateSubtractMode();
                HandleSubtractInput();
            }
        }

        /// <summary>�R���|�[�l���g������</summary>
        private void InitializeComponents()
        {
            // �K�v�ȃR���|�[�l���g�̑��݊m�F
            if (skill == null) Debug.LogError("SubtractSkill component not found!");
            if (targetDetector == null) Debug.LogError("SubtractTargetDetector component not found!");
        }

        /// <summary>�C�x���g���X�i�[�ݒ�</summary>
        private void SetupEventListeners()
        {
            if (skill != null)
            {
                skill.OnSkillActivated += OnSkillActivated;
                skill.OnSkillDeactivated += OnSkillDeactivated;
            }

            if (targetDetector != null)
            {
                targetDetector.OnTargetEnter += OnTargetEnter;
                targetDetector.OnTargetExit += OnTargetExit;
                targetDetector.OnTargetHover += OnTargetHover;
            }
        }

        /// <summary>�X�L���������̍X�V����</summary>
        private void UpdateSubtractMode()
        {
            // �^�[�Q�b�g���m�X�V
            targetDetector?.UpdateTargets();

            // UI�X�V
            UpdateModeUI();
            UpdateCursor();
            UpdateHighlights();
        }

        /// <summary>�X�L���������̓��͏���</summary>
        private void HandleSubtractInput()
        {
            if (!EnableClick) return;

            if (Input.GetMouseButtonDown(0)) // ���N���b�N
            {
                if (TryRemoveCurrentTarget())
                {
                    onRemoveSuccess?.Invoke();
                }
            }
        }

        /// <summary>���݂̃^�[�Q�b�g���폜���s</summary>
        public bool TryRemoveCurrentTarget()
        {
            if (!IsSubtractMode) return false;

            return targetDetector?.TryRemoveHoverTarget() == true;
        }

        /// <summary>���[�hUI�X�V</summary>
        private void UpdateModeUI()
        {
            if (modeUI == null || skill == null) return;

            modeUI.UpdateTimer(skill.RemainingSkillTime, skill.SkillDuration);
        }

        /// <summary>�J�[�\���X�V</summary>
        private void UpdateCursor()
        {
            if (cursor == null) return;

            bool hasTarget = CurrentTarget != null;

            if (hasTarget && !cursor.IsTargetMode())
            {
                cursor.SetTargetCursor();
            }
            else if (!hasTarget && cursor.IsTargetMode())
            {
                cursor.SetNormalCursor();
            }
        }

        /// <summary>�n�C���C�g�X�V</summary>
        private void UpdateHighlights()
        {
            if (highlight == null || targetDetector == null) return;

            highlight.HighlightTargets(targetDetector.DetectedTargets);

            if (CurrentTarget != null)
            {
                highlight.SetHoverTarget(CurrentTarget);
            }
        }

        #region Event Handlers

        private void OnSkillActivated()
        {
            // UI�\���J�n
            modeUI?.SetActive(true);

            // �����^�[�Q�b�g���m
            targetDetector?.UpdateTargets();
        }

        private void OnSkillDeactivated()
        {
            // UI��\��
            modeUI?.SetActive(false);

            // �J�[�\�����Z�b�g
            cursor?.SetNormalCursor();

            // �n�C���C�g�N���A
            highlight?.ClearHighlights();
        }

        private void OnTargetEnter(RemovableObject target)
        {

        }

        private void OnTargetExit(RemovableObject target)
        {

        }

        private void OnTargetHover(RemovableObject target)
        {
            // �z�o�[���̏����i�K�v�ɉ����āj
        }

        #endregion

        #region Public Methods

        /// <summary>�X�L�������i�O������̌Ăяo���p�j</summary>
        public void ActivateSkill()
        {
            skill?.ActivateSkill();
        }

        /// <summary>�X�L���I���i�O������̌Ăяo���p�j</summary>
        public void DeactivateSkill()
        {
            skill?.DeactivateSkill();
        }

        /// <summary>�N���b�N�L��/�����؂�ւ�</summary>
        public void SetClickEnabled(bool enabled)
        {
            EnableClick = enabled;
        }

        /// <summary>���݂̏�Ԏ擾</summary>
        public SubtractStatus GetStatus()
        {
            return new SubtractStatus
            {
                IsActive = IsSubtractMode,
                RemainingTime = skill?.RemainingSkillTime ?? 0f,
                TargetCount = targetDetector?.DetectedTargets?.Count ?? 0,
                CurrentTarget = CurrentTarget?.name
            };
        }

        #endregion

        void OnDestroy()
        {
            // �C�x���g����
            if (skill != null)
            {
                skill.OnSkillActivated -= OnSkillActivated;
                skill.OnSkillDeactivated -= OnSkillDeactivated;
            }

            if (targetDetector != null)
            {
                targetDetector.OnTargetEnter -= OnTargetEnter;
                targetDetector.OnTargetExit -= OnTargetExit;
                targetDetector.OnTargetHover -= OnTargetHover;
            }
        }
    }

    /// <summary>�V�X�e����ԏ��</summary>
    public struct SubtractStatus
    {
        public bool IsActive;
        public float RemainingTime;
        public int TargetCount;
        public string CurrentTarget;
    }
}