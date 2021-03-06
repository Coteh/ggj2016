﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyBehaviour : MonoBehaviour {
    public Transform playerTarget;
    public List<EnemyBehaviour> allEnemies;
    Vector2 velocity;
    bool shouldSeek;
    float maxVelocity;
    float avoidanceRadius;
    float seekDistance;
    float mass;
    const float ATTACK_DELAY = 1.1f;
    float attackCountdown = 0;
    HittableBehaviour hittable;
    bool isAlive = true;
    Animator anim;
    SpriteRenderer spriteRenderer;

    void Start() {
        hittable = GetComponent<HittableBehaviour>();
        velocity = Vector2.zero;
        shouldSeek = true;
        avoidanceRadius = 3f;
        maxVelocity = 5.0f;
        seekDistance = 15.0f;
        mass = 20.0f;
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
	}
	
	void Update () {
        if (!isAlive) return;
        float distance = Vector3.Distance(playerTarget.transform.position, transform.position);
        if (distance < seekDistance && distance > 2) {
            seek(playerTarget.position);
            anim.Play("skeleton_walk");
            attackCountdown = 0;
            if (velocity.x < 0) {
                spriteRenderer.flipX = true;
            } else {
                spriteRenderer.flipX = false;
            }
        } else if (distance <= 2) {
            updateAttack();
            velocity = Vector2.zero;
            anim.Play("skeleton_attack");
            if (playerTarget.transform.position.x < transform.position.x) {
                spriteRenderer.flipX = true;
            } else {
                spriteRenderer.flipX = false;
            }
        } else {
            velocity = Vector2.zero;
            attackCountdown = 0;
            anim.Play("skeleton_idle");
        }

    }


    void updateAttack() {
        if (attackCountdown <= 0) {
            playerTarget.GetComponent<HittableBehaviour>().Damage(10);
            attackCountdown = ATTACK_DELAY;
        } else {
            attackCountdown -= Time.deltaTime;
        }
    }

    void seek(Vector3 target) {
        Vector2 dir = target - transform.position;

        Vector2 desiredVelocity = dir.normalized * maxVelocity;
        Vector2 steering = desiredVelocity - velocity;
        steering += collisionAvoidance(new Vector2(0, 0), 1f, dir);
        steering += collisionAvoidance(new Vector2(0, 0), 1f, Quaternion.Euler(0,0, 60f) * dir);
        steering += collisionAvoidance(new Vector2(0, 0), 1f, Quaternion.Euler(0, 0, -60f) * dir);
        float maxForce = 1000.0f;
        steering = truncate(steering, maxForce);
        steering = steering / mass;

        velocity = truncate(velocity + steering, maxVelocity); 

        move();
    }

    Vector2 collisionAvoidance(Vector2 rayOriginOffset, float rayLength, Vector2 dir) {
        Vector2 avoidance = Vector2.zero;

        for (int i = 0; i < allEnemies.Count; i++) {
            if (this != allEnemies[i]) {
                float distance = Vector2.Distance(transform.position, allEnemies[i].transform.position);
                if (distance < avoidanceRadius) {
                    float maxAvoidForce = 3.0f;
                    avoidance.x = transform.position.x + velocity.x - allEnemies[i].transform.position.x;
                    avoidance.y = transform.position.y + velocity.y - allEnemies[i].transform.position.y;
                    avoidance.Normalize();
                    avoidance *= maxAvoidForce;
                }
            }
        }

        return avoidance;
    }

    Vector3 truncate(Vector3 v, float max) {
        return (v.magnitude > max) ? v.normalized * max : v;
    }

    void OnDead() {
        if (!isAlive) return;
        anim.Play("no_animation");
        isAlive = false;
        transform.Rotate(0,0,90);
    }

    void move() {
        transform.position += (Vector3)velocity * Time.deltaTime;
    }
}
