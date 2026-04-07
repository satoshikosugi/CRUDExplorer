import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Query;
import org.springframework.data.repository.query.Param;
import java.util.List;

public interface UserRepository extends JpaRepository<User, Long> {

    @Query(value = "SELECT * FROM users WHERE email = ?1", nativeQuery = true)
    User findByEmail(String email);

    @Query(value = "SELECT * FROM users WHERE id = :id", nativeQuery = true)
    User findById(@Param("id") Long id);

    @Query(value = "SELECT o.order_id, o.total FROM orders o WHERE o.user_id = :userId", nativeQuery = true)
    List<Order> findOrdersByUserId(@Param("userId") Long userId);

    @Query(value = "SELECT p.id, p.name FROM products p JOIN order_items oi ON p.id = oi.product_id WHERE oi.order_id = :orderId", nativeQuery = true)
    List<Product> findProductsByOrderId(@Param("orderId") Long orderId);
}
