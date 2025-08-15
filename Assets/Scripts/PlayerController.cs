using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float gravity = -20f;

    [Header("Collisions")]
    [SerializeField] private LayerMask collideWith;
    [SerializeField] private int verticalRayAmount = 4;

    #region Internal

    private BoxCollider2D _boxCollider2D;

    private Vector2 _boundsTopLeft;
    private Vector2 _boundsTopRight;
    private Vector2 _boundsBottomLeft;
    private Vector2 _boundsBottomRight;

    private float _boundsWidth;
    private float _boundsHeight;

    private float _currentGravity;
    private Vector2 _force;
    private Vector2 _movePosition;
    private float _skin = 0.05f;

    #endregion

    private void Start()
    {
        _boxCollider2D = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        ApplyGravity();
        StartMovement();

        SetRayOrigins();
        CollisionBelow();

        transform.Translate(_movePosition, Space.Self);

        SetRayOrigins();
        CalculateMovement();
    }

    #region Collision

    #region Collision Below

    private void CollisionBelow()
    {
        // Calculate ray lenght
        float rayLenght = _boundsHeight / 2f + _skin;
        if (_movePosition.y < 0)
        {
            rayLenght += Mathf.Abs(_movePosition.y);
        }

        // Calculate ray origin
        Vector2 leftOrigin = (_boundsBottomLeft + _boundsTopLeft) / 2f;
        Vector2 rightOrigin = (_boundsBottomRight + _boundsTopRight) / 2f;
        leftOrigin += (Vector2)(transform.up * _skin) + (Vector2)(transform.right * _movePosition.x);
        rightOrigin += (Vector2)(transform.up * _skin) + (Vector2)(transform.right * _movePosition.x);

        // Raycast
        for (int i = 0; i < verticalRayAmount; i++)
        {
            Vector2 rayOrigin = Vector2.Lerp(leftOrigin, rightOrigin, (float)i / (float)(verticalRayAmount - 1));
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, -transform.up, rayLenght, collideWith);
            Debug.DrawRay(rayOrigin, -transform.up * rayLenght, Color.green);

            if (hit)
            {
                _movePosition.y = -hit.distance + _boundsHeight / 2f + _skin;

                if (Mathf.Abs(_movePosition.y) < 0.0001f)
                {
                    _movePosition.y = 0f;
                }
            }
        }
    }

    #endregion

    #endregion

    #region Movement

    // Clamp our force applied
    private void CalculateMovement()
    {
        if (Time.deltaTime > 0)
        {
            _force = _movePosition / Time.deltaTime;
        }
    }

    // Initialize the movePosition
    private void StartMovement()
    {
        _movePosition = _force * Time.deltaTime;
    }

    // Calculate the gravity to apply
    private void ApplyGravity()
    {
        _currentGravity = gravity;

        _force.y += _currentGravity * Time.deltaTime;
    }

    #endregion

    #region Ray Origins

    // Calculate ray based on our collider
    private void SetRayOrigins()
    {
        Bounds playerBounds = _boxCollider2D.bounds;

        _boundsBottomLeft = new Vector2(playerBounds.min.x, playerBounds.min.y);
        _boundsBottomRight = new Vector2(playerBounds.max.x, playerBounds.min.y);
        _boundsTopLeft = new Vector2(playerBounds.min.x, playerBounds.max.y);
        _boundsTopRight = new Vector2(playerBounds.max.x, playerBounds.max.y);

        _boundsHeight = Vector2.Distance(_boundsBottomLeft, _boundsTopLeft);
        _boundsWidth = Vector2.Distance(_boundsBottomLeft, _boundsBottomRight);
    }

    #endregion
}
