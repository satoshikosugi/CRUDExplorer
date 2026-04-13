<?php

class UserRepository {
    private PDO $pdo;

    public function __construct(PDO $pdo) {
        $this->pdo = $pdo;
    }

    public function findUser(int $id): ?array {
        $stmt = $this->pdo->prepare("SELECT id, name, email FROM users WHERE id = :id");
        $stmt->execute([':id' => $id]);
        return $stmt->fetch(PDO::FETCH_ASSOC) ?: null;
    }

    public function createUser(string $name, string $email): void {
        $stmt = $this->pdo->prepare("INSERT INTO users (name, email) VALUES (:name, :email)");
        $stmt->execute([':name' => $name, ':email' => $email]);
    }

    public function updateUser(int $id, string $email): void {
        $stmt = $this->pdo->prepare("UPDATE users SET email = :email WHERE id = :id");
        $stmt->execute([':email' => $email, ':id' => $id]);
    }

    public function deleteOrder(int $id): void {
        $stmt = $this->pdo->prepare("DELETE FROM orders WHERE id = :id");
        $stmt->execute([':id' => $id]);
    }
}
