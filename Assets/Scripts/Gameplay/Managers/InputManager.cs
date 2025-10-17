using Core;
using System;
using UnityEngine;

namespace Gameplay
{
    public class InputManager : Injectable<InputManager>
    {
    
        public MyInput config; 

        public event Action OnDodgePressed;
        public event Action OnAttackPressed;
        public event Action OnMovePressed;
        public event Action OnPausePressed;
        public event Action OnSwitchWeaponPressed;
        public event Action OnAbility1Pressed;
        public event Action OnAbility2Pressed;
        public event Action OnAbility3Pressed;

        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(gameObject);
        }

        private void Update()
        {
            HandleInput();
        }

        private void HandleInput()
        {
            if (config == null) return;

            if (Input.GetKeyDown(config.dodge)) OnDodgePressed?.Invoke();
            if (Input.GetKeyDown(config.attack)) OnAttackPressed?.Invoke();
            if (Input.GetKeyDown(config.move)) OnMovePressed?.Invoke();
            if (Input.GetKeyDown(config.pause)) OnPausePressed?.Invoke();
            if (Input.GetKeyDown(config.switchWeapon)) OnSwitchWeaponPressed?.Invoke();
            if (Input.GetKeyDown(config.ability_1)) OnAbility1Pressed?.Invoke();
            if (Input.GetKeyDown(config.ability_2)) OnAbility2Pressed?.Invoke();
            if (Input.GetKeyDown(config.ability_3)) OnAbility3Pressed?.Invoke();
        }

        public void RebindKey(string actionName, KeyCode newKey)
        {
            if (config == null) return;

            switch (actionName)
            {
                case "Dodge": config.dodge = newKey; break;
                case "Attack": config.attack = newKey; break;
                case "Move": config.move = newKey; break;
                case "Pause": config.pause = newKey; break;
                case "SwitchWeapon": config.switchWeapon = newKey; break;
                case "Ability1": config.ability_1 = newKey; break;
                case "Ability2": config.ability_2 = newKey; break;
                case "Ability3": config.ability_3 = newKey; break;
                default: Debug.LogWarning($"[InputManager] Azione '{actionName}' non trovata!"); break;
            }
        }
    }
}