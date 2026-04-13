import psycopg2

def get_user(conn, user_id):
    cursor = conn.cursor()
    cursor.execute("SELECT id, name, email FROM users WHERE id = %s", (user_id,))
    return cursor.fetchone()

def create_user(conn, name, email):
    cursor = conn.cursor()
    cursor.execute("INSERT INTO users (name, email) VALUES (%s, %s)", (name, email))
    conn.commit()

def update_user_email(conn, user_id, email):
    cursor = conn.cursor()
    cursor.execute("UPDATE users SET email = %s WHERE id = %s", (email, user_id))
    conn.commit()

def delete_order(conn, order_id):
    cursor = conn.cursor()
    cursor.execute("DELETE FROM orders WHERE id = %s", (order_id,))
    conn.commit()
