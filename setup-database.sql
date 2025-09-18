-- Database setup script for Mini Payment Gateway
-- Run this script in MySQL to create the database

-- Create the database
CREATE DATABASE IF NOT EXISTS mini_payment_gateway;

-- Use the database
USE mini_payment_gateway;

-- Show tables (will be empty initially, EF will create them)
SHOW TABLES;

-- Optional: Create a user with specific permissions (if needed)
-- CREATE USER 'payment_user'@'localhost' IDENTIFIED BY 'your_password';
-- GRANT ALL PRIVILEGES ON mini_payment_gateway.* TO 'payment_user'@'localhost';
-- FLUSH PRIVILEGES;
