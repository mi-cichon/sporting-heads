using System;
using System.Collections;
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
        private GameObject _playerHand;
        private GameObject _playerHandPivot;
        private Slider _cooldownSlider;
        private Image _cooldownFill;
        
        private readonly Color _cooldownReadyColor = Color.green;
        private readonly Color _cooldownLoadingColor = Color.red;

        private Rigidbody2D _rigidbody;
        private Vector2 _moveInput;
        private bool _isGrounded = false;
        private bool _isOnCooldown = false;

        private float _handRotation = 0.0f;

        private const float MinRotation = 0.0f;
        private const float MaxRotation = 180.0f;
        
        private const string JumpableLayerName = "Jumpable";
        private const string PlayerHandTag = "PlayerHand";
        private const string PlayerHandPivotTag = "PlayerHandPivot";
        
        private const string Player1InputMapName = "Player1";
        private const string Player2InputMapName = "Player2";
        
        private const string Player1CooldownTag = "Player1Cooldown";
        private const string Player2CooldownTag = "Player2Cooldown";
        private const string CooldownFillTag = "CooldownFill";

        void Start()
        {
            _jumpableLayer = LayerMask.NameToLayer(JumpableLayerName);
            _rigidbody = gameObject.GetComponent<Rigidbody2D>();
            _playerHand = GetChildWithTagRecursive(transform, PlayerHandTag).gameObject;
            _playerHandPivot = GetChildWithTagRecursive(_playerHand.transform, PlayerHandPivotTag).gameObject;
            
            SetSide();
        }

        void FixedUpdate()
        {
            _rigidbody.linearVelocity = new Vector2(_moveInput.x * Speed * speedModifier, _rigidbody.linearVelocity.y);
            var handRotationAngle = _handRotation * HandRotationSpeed * -1;
            var playerHandRotation = NormalizeAngle(_playerHand.transform.rotation.eulerAngles.z);
            var modificator = isSecondPlayer ? 1 : -1;
            
            if (handRotationAngle > 0 && playerHandRotation < MaxRotation
                || handRotationAngle < 0 && playerHandRotation > MinRotation)
            {
                _playerHand.transform.RotateAround(_playerHandPivot.transform.position, Vector3.forward, modificator * handRotationAngle * Time.deltaTime);
            }
        }

        
        void OnMove(InputValue value)
        {
            _moveInput = value.Get<Vector2>();
        }
        
        void OnJump()
        {
            if (_isGrounded)
            {
                _rigidbody.linearVelocity = new Vector2(_rigidbody.linearVelocity.x, JumpForce);
            }
        }

        void OnHand(InputValue value)
        {
            _handRotation = value.Get<float>() == 0 
                ? -1
                : 1;
        }

        void OnPower()
        {
            
            if (_isOnCooldown)
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
                _isGrounded = true;
            }
        }

        void OnCollisionExit2D(Collision2D collision)
        {
            if (collision.gameObject.layer == _jumpableLayer)
            {
                _isGrounded = false;
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

            var playerInput = gameObject.AddComponent<PlayerInput>();
            playerInput.actions = inputActionAsset;
            playerInput.SwitchCurrentActionMap(actionMapName);
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

