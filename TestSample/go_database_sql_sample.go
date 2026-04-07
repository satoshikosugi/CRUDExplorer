package repository

import (
	"context"
	"database/sql"
)

type UserRepository struct {
	db *sql.DB
}

func (r *UserRepository) GetUser(ctx context.Context, userID int) (*User, error) {
	row := r.db.QueryContext(ctx, "SELECT id, name, email FROM users WHERE id = $1", userID)
	return scanUser(row)
}

func (r *UserRepository) CreateUser(ctx context.Context, name, email string) error {
	_, err := r.db.ExecContext(ctx, "INSERT INTO users (name, email) VALUES ($1, $2)", name, email)
	return err
}

func (r *UserRepository) UpdateUserEmail(ctx context.Context, userID int, email string) error {
	_, err := r.db.ExecContext(ctx, "UPDATE users SET email = $1 WHERE id = $2", email, userID)
	return err
}

func (r *UserRepository) DeleteOrder(ctx context.Context, orderID int) error {
	_, err := r.db.ExecContext(ctx, "DELETE FROM orders WHERE id = $1", orderID)
	return err
}
