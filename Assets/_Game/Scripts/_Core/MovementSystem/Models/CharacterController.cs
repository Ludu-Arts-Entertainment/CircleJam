using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour, IPoolObject
{
    [SerializeField] private AnimationController _animationController;
    [SerializeField] private SkinnedMeshRenderer _skinnedMeshRenderer;

    private Collider collider;
    public Collider Collider => collider ??= GetComponent<Collider>();

    public GridNode CurrentGridNode => _currentGridNode;

    public string PoolId { get => "Character" ; set => throw new System.NotImplementedException(); }

    private GridNode _currentGridNode;
    private GoalColor _color;
    public GoalColor Color => _color;
    
    public void Initialize(GoalColor color, GridNode gridNode)
    {
        _color = color;
        _currentGridNode = gridNode;

        var characterData = CharacterDataService.GetCharacterByColor(color);
        _skinnedMeshRenderer.material = characterData.Material;

        _animationController.PlayAnimation(AnimationType.Idle);
    }

    public void OnSpawned()
    {
        GameInstaller.Instance.SystemLocator.EventManager.Subscribe<Events.DragStarted>(OnDragStarted);
        GameInstaller.Instance.SystemLocator.EventManager.Subscribe<Events.DragStopped>(OnDragStopped);
    }

    public void OnRecycled()
    {
        GameInstaller.Instance.SystemLocator.EventManager.Unsubscribe<Events.DragStarted>(OnDragStarted);
        GameInstaller.Instance.SystemLocator.EventManager.Unsubscribe<Events.DragStopped>(OnDragStopped);
    }

    private void OnDragStopped(Events.DragStopped stopped)
    {
        Collider.enabled = true;
    }

    private void OnDragStarted(Events.DragStarted started)
    {
        Collider.enabled = false;
    }
}