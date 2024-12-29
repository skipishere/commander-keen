using System;
using Godot;

public interface IState<T> where T : struct, System.Enum
{
    public T StateType { get; }

    public bool CanMove {get;}

    public CharacterBody2D player { get; set; }
    public T? NextState { get; }

    void StateInput(InputEvent inputEvent);

    void PhysicsProcess(double delta, float lastMovementX);

    void Enter();

    void Exit();
}
