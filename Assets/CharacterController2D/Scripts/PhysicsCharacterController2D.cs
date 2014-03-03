#define DEBUG
using UnityEngine;
using System;
using System.Collections.Generic;


namespace CharacterController
{
	[RequireComponent( typeof( Collider2D ), typeof( Rigidbody2D ) )]
	public class PhysicsCharacterController2D : MonoBehaviour
	{
		#region Properties and Fields

		/// <summary>
		/// mask with all layers that the player should interact with
		/// </summary>
		public LayerMask platformMask = 0;

		/// <summary>
		/// mask with all layers that should act as one-way platforms. Note that one-way platforms should always be EdgeCollider2Ds
		/// </summary>
		public LayerMask oneWayPlatformMask = 0;

		[HideInInspector]
		public new Transform transform;
		[HideInInspector]
		public new Collider2D collider2D;
		[HideInInspector]
		public Rigidbody2D rigidBody2D;

		[HideInInspector]
		[NonSerialized]
		public CharacterCollisionState2D collisionState = new CharacterCollisionState2D();
		public bool isGrounded { get { return collisionState.below; } }

		// rigidbody2D.velocity appears to be highly inaccurate when a collision occurs. We will instead handle calculating
		// velocity ourselves by storing the position before moving then marking the velocity as dirty so the property
		// recalculates it.
		private Vector3 _previousPosition;
		private bool _velocityIsDirty;
		private Vector3 _velocity;
		public Vector3 velocity
		{
			get
			{
				if( _velocityIsDirty )
				{
					_velocity = ( transform.position - _previousPosition ) / Time.fixedDeltaTime;
					_velocityIsDirty = false;
				}

				return _velocity;
			}
			set
			{
				_velocity = value;
			}
		}

		#endregion


		#region Monobehaviour

		void Awake()
		{
			// add our one-way platforms to our normal platform mask so that we can land on them from above
			platformMask |= oneWayPlatformMask;

			// cache some components
			transform = GetComponent<Transform>();
			collider2D = GetComponent<Collider2D>();
			rigidBody2D = GetComponent<Rigidbody2D>();

			_previousPosition = transform.position;

			// TODO: setup collisions and one-way platforms
			Physics2D.IgnoreLayerCollision( gameObject.layer, 30 );
		}


		void OnCollisionEnter2D( Collision2D col )
		{

		}
		
		
		void OnCollisionExit2D( Collision2D col )
		{
			// TODO: probably need to manage and store all collisions so we can detect when we are no longer grounded
			collisionState.below = false;
		}


		void OnCollisionStay2D( Collision2D col )
		{
			foreach( var c in col.contacts )
			{
				var angle = Mathf.Atan2( c.normal.y - transform.forward.y, c.normal.x - transform.forward.x ) * Mathf.Rad2Deg;
				//Debug.Log( "angle: " + angle );

				if( angle > 45f && angle < 135f )
				{
					collisionState.below = true;
				}
				else if( angle > -135f && angle < -45f )
				{
					collisionState.above = true;
				}
				else if( angle > -225f && angle < -135f )
				{
					collisionState.right = true;
				}
				else if( angle > -45f && angle < 45f )
				{
					collisionState.left = true;
				}
			}
		}

		#endregion


		public void move( Vector3 deltaMovement )
		{
			velocity = ( transform.position - _previousPosition ) / Time.fixedDeltaTime;
			_previousPosition = transform.position;

			// save off our current grounded state
			var wasGroundedBeforeMoving = collisionState.below;
			
			// clear our state
			collisionState.reset();
			_velocityIsDirty = true;

			// move then update our state
#if UNITY_4_5 || UNITY_4_6
			rigidbody2D.MovePosition( transform.position + deltaMovement );
#endif

			// set our becameGrounded state based on the previous and current collision state
			if( !wasGroundedBeforeMoving && collisionState.below )
				collisionState.becameGroundedThisFrame = true;
		}

	}
}
