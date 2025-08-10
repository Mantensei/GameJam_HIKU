using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameJam_HIKU
{
    /// <summary>「ひく」スキルのメイン管理</summary>
    public class SubtractSkill : MonoBehaviour
    {
        [field: SerializeField] public KeyCode ActivateKey { get; private set; } = KeyCode.E;
        [field: SerializeField] public bool CanUseSkill { get; private set; } = true;
        [field: SerializeField] public float CooldownTime { get; private set; } = 1f;
        [field: SerializeField] public float SkillDuration { get; private set; } = 3f;

        private bool isSkillActive = false;
        private float nextUseTime = 0f;
        private float skillEndTime = 0f;
        private float lastDeactivatedTime = 0f;  // 追加：最後に無効化された時刻

        public event System.Action OnSkillActivated;
        public event System.Action OnSkillDeactivated;

        public bool IsSkillActive => isSkillActive;
        public bool IsOnCooldown => Time.unscaledTime < nextUseTime;
        public float RemainingSkillTime => isSkillActive ? Mathf.Max(0f, skillEndTime - Time.unscaledTime) : 0f;

        /// <summary>クールダウンの進行度（0-1）</summary>
        public float CooldownProgress
        {
            get
            {
                if (!IsOnCooldown) return 1f;  // クールダウン完了
                if (CooldownTime <= 0) return 1f;  // ゼロ除算防止

                float elapsed = Time.unscaledTime - lastDeactivatedTime;
                return Mathf.Clamp01(elapsed / CooldownTime);
            }
        }

        void Update()
        {
            SkillInput.HandleInput(this);
            CheckSkillDuration();
        }

        /// <summary>スキル持続時間チェック</summary>
        private void CheckSkillDuration()
        {
            if (isSkillActive && Time.unscaledTime >= skillEndTime)
            {
                DeactivateSkill();
            }
        }

        /// <summary>スキル発動</summary>
        public void ActivateSkill()
        {
            if (!CanUseSkill || IsOnCooldown || isSkillActive) return;
            if (!UIHub.Instance.ShotCounter.TryShoot())
                return;

            isSkillActive = true;
            skillEndTime = Time.unscaledTime + SkillDuration;
            TimeController.StopTime();
            OnSkillActivated?.Invoke();
        }

        /// <summary>スキル終了</summary>
        public void DeactivateSkill()
        {
            if (!isSkillActive) return;

            isSkillActive = false;
            lastDeactivatedTime = Time.unscaledTime;  // 追加：無効化時刻を記録
            nextUseTime = Time.unscaledTime + CooldownTime;
            TimeController.ResumeTime();
            OnSkillDeactivated?.Invoke();
        }
    }

    /// <summary>スキル入力処理</summary>
    public static class SkillInput
    {
        /// <summary>入力処理</summary>
        public static void HandleInput(SubtractSkill skill)
        {
            if (Input.GetKeyDown(skill.ActivateKey))
            {
                if (skill.IsSkillActive)
                {
                    skill.DeactivateSkill();
                }
                else
                {
                    skill.ActivateSkill();
                }
            }
        }
    }

    /// <summary>時間制御システム</summary>
    public static class TimeController
    {
        private static float originalTimeScale = 1f;
        private static bool isTimeStopped = false;

        public static bool IsTimeStopped => isTimeStopped;

        /// <summary>時間停止</summary>
        public static void StopTime()
        {
            if (isTimeStopped) return;

            originalTimeScale = Time.timeScale;
            Time.timeScale = 0f;
            isTimeStopped = true;
        }

        /// <summary>時間再開</summary>
        public static void ResumeTime()
        {
            if (!isTimeStopped) return;

            Time.timeScale = originalTimeScale;
            isTimeStopped = false;
        }

        /// <summary>元の時間スケールを設定</summary>
        public static void SetOriginalTimeScale(float timeScale)
        {
            originalTimeScale = timeScale;
            if (!isTimeStopped)
            {
                Time.timeScale = timeScale;
            }
        }
    }
}