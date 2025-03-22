using System;
using System.Collections;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Characters.Scripts
{
    public abstract class CharacterBase : MonoBehaviour
    {
        public abstract string CharacterName { get; }
        
        public abstract string CharacterDescription { get; }
        
        public abstract string SuperPowerDescription { get; }
        
        public abstract float SuperPowerCooldown { get; }
        
        protected abstract Action UseSuperPower { get; }
        
        protected abstract float Speed { get; }
        protected abstract float JumpForce { get; }
        protected abstract float HandRotationSpeed { get; }

        public float speedModifier = 1.0f;

        public bool isSecondPlayer = false;
        public Vector3 defaultPosition;
        public InputActionAsset inputActionAsset;
        
        private int _jumpableLayer;
        private int _playerLayer;
        private int _jumpPadLayer;
        private int _ballLayer;

        private PlayerInput _playerInput;

        [CanBeNull] private JumpPadController _useJumpPadController;
        
        private GameObject _playerHand;
        private GameObject _playerHandPivot;
        private Slider _cooldownSlider;
        private Image _cooldownFill;

        private bool _holdingJump = false;
        
        private readonly Color _cooldownReadyColor = Color.green;
        private readonly Color _cooldownLoadingColor = Color.red;

        private Rigidbody2D _rigidbody;
        private Vector2 _moveInput;
        private bool _isGrounded = false;
        private bool _isOnCooldown = false;
        private int _groundContacts = 0;

        private bool _freezeInputs = false;

        private float _handRotation = 0.0f;

        private const float MinRotation = 20.0f;
        private const float MaxRotation = 160.0f;
        
        private const string JumpableLayerName = "Jumpable";
        private const string PlayerLayerName = "Player";
        private const string BallLayerName = "Ball";
        private const string PlayerHandTag = "PlayerHand";
        private const string PlayerHandPivotTag = "PlayerHandPivot";

        private const string JumpPadLayerName = "JumpPad";
        
        private const string Player1InputMapName = "Player1";
        private const string Player2InputMapName = "Player2";
        
        private const string Player1CooldownTag = "Player1Cooldown";
        private const string Player2CooldownTag = "Player2Cooldown";
        private const string CooldownFillTag = "CooldownFill";

        void Start()
        {
            _playerLayer = LayerMask.NameToLayer(PlayerLayerName);
            _ballLayer = LayerMask.NameToLayer(BallLayerName);
            _jumpableLayer = LayerMask.NameToLayer(JumpableLayerName);
            _jumpPadLayer = LayerMask.NameToLayer(JumpPadLayerName);
            _rigidbody = gameObject.GetComponent<Rigidbody2D>();
            _playerHand = GetChildWithTagRecursive(transform, PlayerHandTag).gameObject;
            _playerHandPivot = GetChildWithTagRecursive(_playerHand.transform, PlayerHandPivotTag).gameObject;
            
            SetSide();
        }

        void FixedUpdate()
        {
            if (!_freezeInputs)
            {
                var movementForce = new Vector2(_moveInput.x * Speed * speedModifier - _rigidbody.linearVelocity.x, 0);
            
                _rigidbody.AddForce(movementForce, ForceMode2D.Impulse);
            }
            
            var handRotationValue = _freezeInputs
                ? -1
                : _handRotation;
            
            var handRotationAngle = handRotationValue * HandRotationSpeed * -1;
            var playerHandRotation = NormalizeAngle(_playerHand.transform.rotation.eulerAngles.z);
            var modificator = isSecondPlayer ? 1 : -1;

            if (handRotationAngle > 0 && playerHandRotation < MaxRotation
                || handRotationAngle < 0 && playerHandRotation > MinRotation)
            {
                _playerHand.transform.RotateAround(_playerHandPivot.transform.position, Vector3.forward, modificator * handRotationAngle * Time.deltaTime);
            }

            if (_holdingJump && _isGrounded && !_freezeInputs)
            {
                if (_useJumpPadController)
                {
                    var playerLaunched = _useJumpPadController.LaunchPlayer(_rigidbody);
                    if (playerLaunched)
                    {
                        _rigidbody.AddForce(Vector2.up * _useJumpPadController.jumpPadLaunchForce, ForceMode2D.Impulse);
                    }

                    _useJumpPadController = null;
                    return;
                }
                
                _rigidbody.AddForce(Vector2.up * JumpForce, ForceMode2D.Impulse);
            }
        }

        
        void OnMove(InputValue value)
        {
            _moveInput = value.Get<Vector2>();
        }
        
        void OnJump(InputValue value)
        {
            var axis = value.Get<float>();
            if (axis > 0)
            {
                _holdingJump = true;
                return;
            }
            _holdingJump = false;
        }

        void OnHand(InputValue value)
        {
            _handRotation = value.Get<float>() == 0 
                ? -1
                : 1;
        }

        void OnPower()
        {
            
            if (_isOnCooldown || _freezeInputs)
            {
                return;
            }
            
            StartCooldown();
            UseSuperPower.Invoke();
        }
        
        void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.layer == _jumpableLayer)
            {
                _groundContacts++;
                _isGrounded = _groundContacts > 0;
                return;
            }

            if (collision.gameObject.layer == _jumpPadLayer)
            {
                _groundContacts++;
                _isGrounded = _groundContacts > 0;
                _useJumpPadController = collision.gameObject.GetComponentInParent<JumpPadController>();
                return;
            }

            if (collision.gameObject.layer == _ballLayer)
            {
                collision.rigidbody.AddForce(Vector2.up * 0.2f, ForceMode2D.Impulse);
                return;
            }
        }

        void OnCollisionExit2D(Collision2D collision)
        {
            if (collision.gameObject.layer == _jumpableLayer)
            {
                _groundContacts--;
                _isGrounded = _groundContacts > 0;
                return;
            }

            if (collision.gameObject.layer == _jumpPadLayer)
            {
                _groundContacts--;
                _isGrounded = _groundContacts > 0;
                _useJumpPadController = null;
                return;
            }
        }
        
        float NormalizeAngle(float angle)
        {
            if (angle > 180)
                return NormalizeAngle(angle - 360);
            if (angle < -180)
                return NormalizeAngle(angle + 360);
            return angle;
        }

        public void MoveToDefaultPosition()
        {
            transform.position = defaultPosition;
        }

        public void SetInput()
        {
            var actionMapName = isSecondPlayer
                ? Player2InputMapName
                : Player1InputMapName;

            _playerInput = gameObject.AddComponent<PlayerInput>();
            _playerInput.actions = inputActionAsset;
            _playerInput.SwitchCurrentActionMap(actionMapName);
        }

        public void SetSlider()
        {
            var sliderTag = isSecondPlayer
                ? Player2CooldownTag
                : Player1CooldownTag;

            var sliderGameObject = GameObject.FindWithTag(sliderTag);

            _cooldownSlider = sliderGameObject.GetComponent<Slider>();
            _cooldownFill = GetChildWithTagRecursive(sliderGameObject.transform, CooldownFillTag)
                .GetComponent<Image>();
        }
        
        public void FreezeInput(float duration)
        {
            StartCoroutine(FreezeInputCoroutine(duration));
        }

        private IEnumerator FreezeInputCoroutine(float duration)
        {
            _freezeInputs = true;
            yield return new WaitForSeconds(duration);
            _freezeInputs = false;
        }
        
        private void SetSide()
        {
            if (!isSecondPlayer)
            {
                return;
            }
            
            transform.Rotate(0f, 180f, 0f);
            transform.position = new Vector3(transform.position.x, transform.position.y, -2);
            _playerHand.transform.position = new Vector3(_playerHand.transform.position.x, _playerHand.transform.position.y, -1);
        }

        private Transform GetChildWithTagRecursive(Transform parentTransform, string tag)
        {
            foreach (Transform child in parentTransform)
            {
                if (child.CompareTag(tag))
                {
                    return child;
                }

                var foundChild = GetChildWithTagRecursive(child, tag);
                if (foundChild != null)
                {
                    return foundChild;
                }
            }
            return null;
        }

        private void StartCooldown()
        {
            StartCoroutine(CooldownEnumerator());
        }

        private IEnumerator CooldownEnumerator()
        {
            _isOnCooldown = true;
            var cooldownTimer = SuperPowerCooldown;
            _cooldownFill.color = _cooldownLoadingColor;

            while (cooldownTimer > 0)
            {
                cooldownTimer -= Time.deltaTime;
                _cooldownSlider.value = 1 - cooldownTimer / SuperPowerCooldown;
                yield return null;
            }

            _cooldownFill.color = _cooldownReadyColor;
            _cooldownSlider.value = 1;
            _isOnCooldown = false;
        }
    }
}

