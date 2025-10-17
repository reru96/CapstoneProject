using System.Collections;
using System.Collections.Generic;
using Core;
using Gameplay;
using UnityEngine;

public class PlayerSwitchWeaponState : PlayerBaseState
{
    private int _direction; 
    private float _switchDuration = 0.4f; 
    private float _timer;
    private bool _weaponSwitched;
    private bool isMoving;

    private readonly string _upperBodyLayerName = "UpperBody";


    private InventoryManager _inventory;

    public PlayerSwitchWeaponState(PlayerStateMachine player, int direction) : base(player)
    {
        _direction = direction;
    }

    public override void Enter()
    {
        player.SetUpperBodyActive(true);
        _inventory = ServiceLocator.Get<InventoryManager>();

        _timer = 0f;
        _weaponSwitched = false;
        int layerIndex = player.animator.GetLayerIndex(_upperBodyLayerName);

        player.animator.Play("SwitchWeapon", layerIndex, 0f);
    }

    public override void Tick()
    {
        HandleMovement();

        _timer += Time.deltaTime;

        if (!_weaponSwitched && _timer >= _switchDuration * 0.5f)
        {
            var newWeapon = CycleWeapon(_direction);
            if (newWeapon != null)
                player.EquipNewWeapon(newWeapon);

            _weaponSwitched = true;
        }


        if (_timer >= _switchDuration)
        {

            if (isMoving)
                player.SwitchState(new PlayerMoveState(player));
            else
                player.SwitchState(new PlayerIdleState(player));
        }
    }

    public override void Exit()
    {
      player.SetUpperBodyActive(false);
    }

    private void HandleMovement()
    {
        Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        if (input.sqrMagnitude > 0.01f)
        {
            isMoving = true;
            float moveSpeedMultiplier = 0.8f;
            player.rb.velocity = input.normalized * (player.agent.speed * moveSpeedMultiplier);

            Quaternion targetRotation = Quaternion.LookRotation(input.normalized, Vector3.up);
            player.rb.MoveRotation(Quaternion.Slerp(player.rb.rotation, targetRotation, Time.deltaTime * player.rotationSpeed));
        }
        else
        {
            isMoving = false;
            player.rb.velocity = Vector3.zero;
        }
    }

    private SOWeapon CycleWeapon(int direction)
    {
        var items = _inventory.runInventory.items;
        if (items == null || items.Count == 0)
            return null;

        int currentIndex = items.IndexOf(_inventory.runInventory.equippedItem);
        int nextIndex = (currentIndex + direction + items.Count) % items.Count;

        var nextItem = items[nextIndex];
        _inventory.runInventory.EquipItem(nextItem);

        return nextItem as SOWeapon;
    }
}
