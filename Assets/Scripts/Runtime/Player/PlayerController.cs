using Runtime.Input;
using UnityEngine;

namespace Runtime.Player
{
    public class PlayerController : MonoBehaviour
    {
        #region Header MOVEMENT DETAILS
        [Space(10)]
        [Header("MOVEMENT DETAILS")]
        #endregion

        [SerializeField] private InputReader inputReader;
        [SerializeField] private float moveSpeed = 1f;

        // ============ Movement System ============
        private Rigidbody2D _rb;
        private Animator _movementAnimator;
        private Vector2 _movementInput;
        private static readonly int MoveX = Animator.StringToHash("moveX");
        private static readonly int MoveY = Animator.StringToHash("moveY");
        private static readonly int IsMoving = Animator.StringToHash("isMoving");

        private void Start()
        {
            inputReader.MoveEvent += HandleMove;
            _rb = GetComponent<Rigidbody2D>();
            _movementAnimator = GetComponent<Animator>();
        }

        private void HandleMove(Vector2 direction)
        {
            _movementInput = direction;
        }

        private void FixedUpdate()
        {
            if (_movementInput != Vector2.zero)
            {
                _movementAnimator.SetFloat(id: MoveX, _movementInput.x);
                _movementAnimator.SetFloat(id: MoveY, _movementInput.y);
                _movementAnimator.SetBool(id: IsMoving, true);
            }
            else
            {
                _movementAnimator.SetBool(id: IsMoving, false);
            }
            
            _rb.velocity = _movementInput * moveSpeed;
        }
    }
}