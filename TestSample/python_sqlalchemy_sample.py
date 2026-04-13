from sqlalchemy import text
from sqlalchemy.orm import Session

def get_user(session: Session, user_id: int):
    result = session.execute(
        text("SELECT * FROM users WHERE id = :id"),
        {"id": user_id}
    )
    return result.fetchone()

def create_user(session: Session, name: str, email: str):
    session.execute(
        text("INSERT INTO users (name, email) VALUES (:name, :email)"),
        {"name": name, "email": email}
    )
    session.commit()

def update_user_email(session: Session, user_id: int, email: str):
    session.execute(
        text("UPDATE users SET email = :email WHERE id = :id"),
        {"email": email, "id": user_id}
    )
    session.commit()

def delete_orders_by_user(session: Session, user_id: int):
    session.execute(
        text("DELETE FROM orders WHERE user_id = :user_id"),
        {"user_id": user_id}
    )
    session.commit()
