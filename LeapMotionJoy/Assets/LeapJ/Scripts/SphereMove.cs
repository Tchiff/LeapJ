using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereMove : MonoBehaviour {

    public float TimeBack = 0.2f;
    public AnimationCurve ToBack;
    public float TimeAcceleration = 0.2f;
    public AnimationCurve Acceleration;
    public float Speed = 10f;
    public int waitFrames = 10;

    bool isMaxSpeed;
    float fadedIn = 0f;
    float Velocity = 0f;
    Vector3 Home;
    Vector3 Target;
    Vector3 StartMovePosition;
    MovingMod movingMod;
    int frames_disconnected_ = 0;

    enum MovingMod
    {
        ToTarget,
        ToHome
    }

    void Start ()
    {
        Home = gameObject.transform.position;
        Target = Home;
    }

    public void MoveTo(Vector3 target)
    {
        Target = target;
        frames_disconnected_ = 0;
        if (movingMod != MovingMod.ToTarget)
        {
            movingMod = MovingMod.ToTarget;
            ResetParam();
        }
    }

    public void MoveHome()
    {
        Target = Home;
        StartMovePosition = gameObject.transform.position;
        if (movingMod != MovingMod.ToHome)
        {
            movingMod = MovingMod.ToHome;
            ResetParam();
        }
    }
	
	void Update ()
    {
        switch(movingMod)
        {
            case MovingMod.ToTarget:
                if(frames_disconnected_ >= waitFrames)
                {
                    MoveHome();
                    break;
                }
                if ((Target - transform.position).sqrMagnitude > 0.01f)
                {
                    AccelerateVelocity(ref Velocity, ref fadedIn);
                    Moving(transform.position, Time.deltaTime * Velocity);
                }
                frames_disconnected_++;
                break;
            case MovingMod.ToHome:
                if ((Target - transform.position).sqrMagnitude > 0.01f)
                {
                    Accelerate(TimeBack, ref fadedIn);
                    Moving(StartMovePosition, fadedIn);
                }
                break;
        }
	}

    void Moving(Vector3 Start, float time)
    {
        transform.position = Vector3.Lerp(transform.position, Target, time);
    }

    void AccelerateVelocity(ref float velocity, ref float timer)
    {
        if (!isMaxSpeed)
        {
            velocity = Accelerate(TimeAcceleration, ref timer) * Speed;
            if (Velocity >= Speed)
            {
                Velocity = Speed;
                isMaxSpeed = true;
            }
        }
    }

    float Accelerate(float Duration, ref float timer)
    {
        timer += Time.deltaTime / Duration;
        timer = Mathf.Clamp(timer, 0f, 1f);
        return Acceleration.Evaluate(fadedIn);
    }

    void ResetParam()
    {
        Velocity = 0f;
        fadedIn = 0f;
        isMaxSpeed = false;
    }
}
