import { Pool, PoolClient } from 'pg';

const pool = new Pool();

async function getUser(userId: number) {
  const result = await pool.query(
    `SELECT id, name, email FROM users WHERE id = $1`,
    [userId]
  );
  return result.rows[0];
}

async function createUser(name: string, email: string) {
  const client: PoolClient = await pool.connect();
  await client.query(
    `INSERT INTO users (name, email) VALUES ($1, $2)`,
    [name, email]
  );
  client.release();
}

async function updateOrderStatus(orderId: number, status: string) {
  const client: PoolClient = await pool.connect();
  await client.query(
    `UPDATE orders SET status = $1 WHERE id = $2`,
    [status, orderId]
  );
  client.release();
}

async function deleteOrderItems(orderId: number) {
  const client: PoolClient = await pool.connect();
  await client.query(
    `DELETE FROM order_items WHERE order_id = $1`,
    [orderId]
  );
  client.release();
}
