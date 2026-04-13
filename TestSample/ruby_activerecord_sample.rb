class UserService
  def find_user(id)
    User.find_by_sql("SELECT * FROM users WHERE id = #{id}").first
  end

  def create_user(name)
    ActiveRecord::Base.connection.execute("INSERT INTO users (name) VALUES ('#{name}')")
  end

  def update_user(id, name)
    ActiveRecord::Base.connection.execute("UPDATE users SET name = '#{name}' WHERE id = #{id}")
  end

  def find_users_with_orders
    User.find_by_sql("SELECT u.id, u.name, o.total FROM users u JOIN orders o ON u.id = o.user_id")
  end
end
