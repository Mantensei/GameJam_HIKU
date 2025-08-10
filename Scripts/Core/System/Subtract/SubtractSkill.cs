using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameJam_HIKU
{
    /// <summary>�u�Ђ��v�X�L���̃��C���Ǘ�</summary>
    public class SubtractSkill : MonoBehaviour
    {
        [field: SerializeField] public KeyCode ActivateKey { get; private set; } = KeyCode.E;
        [field: SerializeField] public bool CanUseSkill { get; private set; } = true;
        [field: SerializeField] public float CooldownTime { get; private set; } = 1f;
        [field: SerializeField] public float SkillDuration { get; private set; } = 3f;

        private bool isSkillActive = false;
        private float nextUseTime = 0f;
        private float skillEndTime = 0f;
        private float lastDeactivatedTime = 0f;  // �ǉ��F�Ō�ɖ��������ꂽ����

        public event System.Action OnSkillActivated;
        public event System.Action OnSkillDeactivated;

        public bool IsSkillActive => isSkillActive;
        public bool IsOnCooldown => Time.unscaledTime < nextUseTime;
        public float RemainingSkillTime => isSkillActive ? Mathf.Max(0f, skillEndTime - Time.unscaledTime) : 0f;

        /// <summary>�N�[���_�E���̐i�s�x�i0-1�j</summary>
        public float CooldownProgress
        {
            get
            {
                if (!IsOnCooldown) return 1f;  // �N�[���_�E������
                if (CooldownTime <= 0) return 1f;  // �[�����Z�h�~

                float elapsed = Time.unscaledTime - lastDeactivatedTime;
                return Mathf.Clamp01(elapsed / CooldownTime);
            }
        }

        void Update()
        {
            SkillInput.HandleInput(this);
            CheckSkillDuration();
        }

        /// <summary>�X�L���������ԃ`�F�b�N</summary>
        private void CheckSkillDuration()
        {
            if (isSkillActive && Time.unscaledTime >= skillEndTime)
            {
                DeactivateSkill();
            }
        }

        /// <summary>�X�L������</summary>
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

        /// <summary>�X�L���I��</summary>
        public void DeactivateSkill()
        {
            if (!isSkillActive) return;

            isSkillActive = false;
            lastDeactivatedTime = Time.unscaledTime;  // �ǉ��F�������������L�^
            nextUseTime = Time.unscaledTime + CooldownTime;
            TimeController.ResumeTime();
            OnSkillDeactivated?.Invoke();
        }
    }

    /// <summary>�X�L�����͏���</summary>
    public static class SkillInput
    {
        /// <summary>���͏���</summary>
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

    /// <summary>���Ԑ���V�X�e��</summary>
    public static class TimeController
    {
        private static float originalTimeScale = 1f;
        private static bool isTimeStopped = false;

        public static bool IsTimeStopped => isTimeStopped;

        /// <summary>���Ԓ�~</summary>
        public static void StopTime()
        {
            if (isTimeStopped) return;

            originalTimeScale = Time.timeScale;
            Time.timeScale = 0f;
            isTimeStopped = true;
        }

        /// <summary>���ԍĊJ</summary>
        public static void ResumeTime()
        {
            if (!isTimeStopped) return;

            Time.timeScale = originalTimeScale;
            isTimeStopped = false;
        }

        /// <summary>���̎��ԃX�P�[����ݒ�</summary>
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