using System;
using Godot;

public interface IState<T, Body> where T : struct, Enum where Body : PhysicsBody2D
{
    public T StateType { get; }

    public bool CanMove { get; }

    public Body Character { get; set; }
    public T? NextState { get; }

    void StateInput(InputEvent inputEvent);

    void PhysicsProcess(double delta, float lastMovementX);

    void Enter();

    void Exit();
}
