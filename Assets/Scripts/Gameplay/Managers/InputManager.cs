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

            SaveBindings();
        }

        public void SaveBindings()
        {
            PlayerPrefs.SetInt("Dodge", (int)config.dodge);
            PlayerPrefs.SetInt("Attack", (int)config.attack);
            PlayerPrefs.SetInt("Move", (int)config.move);
            PlayerPrefs.SetInt("Pause", (int)config.pause);
            PlayerPrefs.SetInt("SwitchWeapon", (int)config.switchWeapon);
            PlayerPrefs.SetInt("Ability1", (int)config.ability_1);
            PlayerPrefs.SetInt("Ability2", (int)config.ability_2);
            PlayerPrefs.SetInt("Ability3", (int)config.ability_3);
            PlayerPrefs.Save();
        }

        public void LoadBindings()
        {
            LoadKey("Dodge", ref config.dodge);
            LoadKey("Attack", ref config.attack);
            LoadKey("Move", ref config.move);
            LoadKey("Pause", ref config.pause);
            LoadKey("SwitchWeapon", ref config.switchWeapon);
            LoadKey("Ability1", ref config.ability_1);
            LoadKey("Ability2", ref config.ability_2);
            LoadKey("Ability3", ref config.ability_3);
        }

        private void LoadKey(string name, ref KeyCode key)
        {
            if (PlayerPrefs.HasKey(name))
                key = (KeyCode)PlayerPrefs.GetInt(name);
        }
    }
}