using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SYBO.Subway.StateSystem;
using UnityEngine.InputSystem;
using SYBO.Subway.Inputs;

namespace Lamb
{
    namespace Game
    {
        public class OtherFunction
        {
            public class NewTouch
            {
                private PlayerInputAction inputActions;
                private Vector2 swipeDelta;
                private Vector2 lastSwipeDelta;
                private SwipeDir lastSwipeDir = SwipeDir.None;
                private float startTime;
                private float replaceTime;
                public float distance;

                public void HandleControls()
                {
                    if (inputActions == null ||
                     inputActions.map.press.phase == InputActionPhase.Disabled ||
                     inputActions.map.press.phase == InputActionPhase.Canceled) return;
                    if (inputActions.map.press.phase == InputActionPhase.Started ||
                     inputActions.map.press.phase == InputActionPhase.Waiting
                    )
                    {
                        swipeDelta = Vector2.zero;
                        startTime = Time.time;
                        lastSwipeDir = SwipeDir.None;
                    }
                    else if (inputActions.map.press.phase == InputActionPhase.Performed)
                    {
                        Vector2 delta = inputActions.map.delta.ReadValue<Vector2>();
                        if (delta != Vector2.zero)
                        {
                            if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
                            {
                                if ((swipeDelta.x > 0 && delta.x < 0) || (swipeDelta.x < 0 && delta.x > 0))
                                {
                                    swipeDelta.x = delta.x;
                                }
                                else
                                {
                                    swipeDelta.x += delta.x;
                                }
                            }
                            else
                            {
                                if ((swipeDelta.y > 0 && delta.y < 0) || (swipeDelta.y < 0 && delta.y > 0))
                                {
                                    swipeDelta.y = delta.y;
                                }
                                else
                                {
                                    swipeDelta.y += delta.y;
                                }
                            }
                            startTime = Time.time;
                        }
                        else
                        {
                            if (Time.time > startTime + replaceTime)
                            {
                                swipeDelta = Vector2.zero;
                                startTime = Time.time;
                                lastSwipeDir = SwipeDir.None;
                            }
                        }
                        JudgeDir();
                    }
                }

                public void JudgeDir()
                {
                    if (swipeDelta.magnitude <= distance) return;
                    SwipeDir swipeDir = SwipeDir.None;
                    //Debug.Log($"magnitude:{swipeDelta.magnitude},swipeDelta.magnitude > distance:{swipeDelta.magnitude > distance}");
                    //Debug.Log($"current:{swipeDir}, last:{lastSwipeDir}");
                    if (Mathf.Abs(swipeDelta.x) > Mathf.Abs(swipeDelta.y))
                    {
                        swipeDir = swipeDelta.x > 0 ? SwipeDir.Right : SwipeDir.Left;
                    }
                    else
                    {
                        swipeDir = swipeDelta.y > 0 ? SwipeDir.Up : SwipeDir.Down;
                    }
                    //Debug.Log($"swipeDir != SwipeDir.None:{swipeDir != SwipeDir.None}, lastSwipeDir == SwipeDir.None:{lastSwipeDir == SwipeDir.None}, swipeDir != lastSwipeDir:{swipeDir != lastSwipeDir}");
                    if (swipeDir != SwipeDir.None && (lastSwipeDir == SwipeDir.None || swipeDir != lastSwipeDir))
                    {
                        //Debug.Log("True swipe");
                        //Debug.Log($"current:{swipeDir}, last:{lastSwipeDir}");
                        GameStateMachine.Instance.HandleSwipe(swipeDir);
                        swipeDelta = Vector2.zero;
                        startTime = Time.time;
                        lastSwipeDir = swipeDir;
                    }
                }

                public void Enable()
                {
                    inputActions?.map.Enable();
                }

                public void Disable()
                {
                    inputActions?.map.Disable();
                }

                public NewTouch(PlayerInputAction inputActions, float proportion, float replaceTime)
                {
                    this.inputActions = inputActions;
                    distance = Mathf.Min(Screen.width, Screen.height) * proportion;
                    this.replaceTime = replaceTime;
                    //Debug.Log(distance);
                }
            }
        }
    }
}