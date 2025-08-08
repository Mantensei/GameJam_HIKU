using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MantenseiLib;
using System;

namespace GameJam_HIKU
{
    /// <summary>「ひく」システム全体の統合管理</summary>
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

        /// <summary>コンポーネント初期化</summary>
        private void InitializeComponents()
        {
            // 必要なコンポーネントの存在確認
            if (skill == null) Debug.LogError("SubtractSkill component not found!");
            if (targetDetector == null) Debug.LogError("SubtractTargetDetector component not found!");
        }

        /// <summary>イベントリスナー設定</summary>
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

        /// <summary>スキル発動中の更新処理</summary>
        private void UpdateSubtractMode()
        {
            // ターゲット検知更新
            targetDetector?.UpdateTargets();

            // UI更新
            UpdateModeUI();
            UpdateCursor();
            UpdateHighlights();
        }

        /// <summary>スキル発動中の入力処理</summary>
        private void HandleSubtractInput()
        {
            if (!EnableClick) return;

            if (Input.GetMouseButtonDown(0)) // 左クリック
            {
                if (TryRemoveCurrentTarget())
                {
                    onRemoveSuccess?.Invoke();
                }
            }
        }

        /// <summary>現在のターゲットを削除試行</summary>
        public bool TryRemoveCurrentTarget()
        {
            if (!IsSubtractMode) return false;

            return targetDetector?.TryRemoveHoverTarget() == true;
        }

        /// <summary>モードUI更新</summary>
        private void UpdateModeUI()
        {
            if (modeUI == null || skill == null) return;

            modeUI.UpdateTimer(skill.RemainingSkillTime, skill.SkillDuration);
        }

        /// <summary>カーソル更新</summary>
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

        /// <summary>ハイライト更新</summary>
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
            // UI表示開始
            modeUI?.SetActive(true);

            // 初期ターゲット検知
            targetDetector?.UpdateTargets();
        }

        private void OnSkillDeactivated()
        {
            // UI非表示
            modeUI?.SetActive(false);

            // カーソルリセット
            cursor?.SetNormalCursor();

            // ハイライトクリア
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
            // ホバー中の処理（必要に応じて）
        }

        #endregion

        #region Public Methods

        /// <summary>スキル発動（外部からの呼び出し用）</summary>
        public void ActivateSkill()
        {
            skill?.ActivateSkill();
        }

        /// <summary>スキル終了（外部からの呼び出し用）</summary>
        public void DeactivateSkill()
        {
            skill?.DeactivateSkill();
        }

        /// <summary>クリック有効/無効切り替え</summary>
        public void SetClickEnabled(bool enabled)
        {
            EnableClick = enabled;
        }

        /// <summary>現在の状態取得</summary>
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
            // イベント解除
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

    /// <summary>システム状態情報</summary>
    public struct SubtractStatus
    {
        public bool IsActive;
        public float RemainingTime;
        public int TargetCount;
        public string CurrentTarget;
    }
}