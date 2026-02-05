CREATE DATABASE IF NOT EXISTS testfleet;
USE testfleet;

-- ADD ON MASTER
CREATE TABLE add_on_master (
    add_on_id INT AUTO_INCREMENT PRIMARY KEY,
    add_on_name VARCHAR(255),
    add_on_daily_rate DOUBLE,
    rate_valid_until DATETIME(6)
);

-- STATE MASTER
CREATE TABLE state_master (
    state_id INT AUTO_INCREMENT PRIMARY KEY,
    state_name VARCHAR(255)
);

-- CITY MASTER
CREATE TABLE city_master (
    city_id INT AUTO_INCREMENT PRIMARY KEY,
    city_name VARCHAR(255),
    state_id INT,
    FOREIGN KEY (state_id) REFERENCES state_master(state_id)
);

-- HUB MASTER
CREATE TABLE hub_master (
    hub_id INT AUTO_INCREMENT PRIMARY KEY,
    hub_name VARCHAR(255),
    hub_address_and_details TEXT,
    contact_number BIGINT UNIQUE,
    city_id INT,
    state_id INT,
    FOREIGN KEY (city_id) REFERENCES city_master(city_id),
    FOREIGN KEY (state_id) REFERENCES state_master(state_id)
);

-- AIRPORT MASTER
CREATE TABLE airport_master (
    airport_id INT AUTO_INCREMENT PRIMARY KEY,
    airport_name VARCHAR(255),
    airport_code VARCHAR(255),
    city_id INT,
    state_id INT,
    hub_id INT,
    FOREIGN KEY (city_id) REFERENCES city_master(city_id),
    FOREIGN KEY (state_id) REFERENCES state_master(state_id),
    FOREIGN KEY (hub_id) REFERENCES hub_master(hub_id)
);

-- CAR TYPE MASTER
CREATE TABLE car_type_master (
    cartype_id BIGINT AUTO_INCREMENT PRIMARY KEY,
    cartype_name VARCHAR(255),
    daily_rate DOUBLE,
    weekly_rate DOUBLE,
    monthly_rate DOUBLE,
    image_path VARCHAR(255)
);

-- CAR MASTER
CREATE TABLE car_master (
    car_id INT AUTO_INCREMENT PRIMARY KEY,
    car_name VARCHAR(255),
    number_plate VARCHAR(50) UNIQUE,
    cartype_id BIGINT,
    hub_id INT,
    is_available VARCHAR(255),
    status VARCHAR(255),
    mileage DOUBLE,
    maintenance_due_date DATE,
    image_path VARCHAR(255),
    FOREIGN KEY (cartype_id) REFERENCES car_type_master(cartype_id),
    FOREIGN KEY (hub_id) REFERENCES hub_master(hub_id)
);

-- CUSTOMER MASTER
CREATE TABLE customer_master (
    cust_id INT AUTO_INCREMENT PRIMARY KEY,
    membership_id VARCHAR(255) UNIQUE,
    first_name VARCHAR(255),
    last_name VARCHAR(255),
    email VARCHAR(255) UNIQUE,
    mobile_number VARCHAR(255),
    address_line1 VARCHAR(255),
    address_line2 VARCHAR(255),
    city VARCHAR(255),
    pincode VARCHAR(255),
    phone_number VARCHAR(255),
    credit_card_type VARCHAR(255),
    credit_card_number VARCHAR(255),
    driving_license_number VARCHAR(255),
    idp_number VARCHAR(255),
    issued_bydl VARCHAR(255),
    valid_throughdl DATE,
    passport_number VARCHAR(255),
    passport_valid_through DATE,
    passport_issued_by VARCHAR(255),
    passport_valid_from DATE,
    passport_issue_date DATE,
    date_of_birth DATE
);

-- BOOKING HEADER
CREATE TABLE booking_header_table (
    booking_id BIGINT AUTO_INCREMENT PRIMARY KEY,
    booking_date DATE,
    confirmation_number VARCHAR(255) UNIQUE,
    booking_status VARCHAR(255),
    cust_id INT NOT NULL,
    start_date DATE,
    end_date DATE,
    pickup_location_id INT,
    return_hub_id INT,
    cartype_id BIGINT,
    car_id INT,
    first_name VARCHAR(255),
    last_name VARCHAR(255),
    address VARCHAR(255),
    state VARCHAR(255),
    pin VARCHAR(255),
    daily_rate DOUBLE,
    weekly_rate DOUBLE,
    monthly_rate DOUBLE,
    email_id VARCHAR(255),
    book_car VARCHAR(255),
    pickup_time DATETIME(6),
    pickup_fuel_status VARCHAR(255),
    pickup_condition VARCHAR(255),
    return_condition VARCHAR(255),
    return_fuel_status VARCHAR(255),
    return_time DATETIME(6),
    FOREIGN KEY (cust_id) REFERENCES customer_master(cust_id),
    FOREIGN KEY (car_id) REFERENCES car_master(car_id)
);

-- BOOKING DETAIL
CREATE TABLE booking_detail_table (
    booking_detail_id BIGINT AUTO_INCREMENT PRIMARY KEY,
    booking_id BIGINT,
    addon_id INT,
    addon_rate DOUBLE,
    FOREIGN KEY (booking_id) REFERENCES booking_header_table(booking_id),
    FOREIGN KEY (addon_id) REFERENCES add_on_master(add_on_id)
);

-- INVOICE HEADER
CREATE TABLE invoice_header_table (
    invoice_id BIGINT AUTO_INCREMENT PRIMARY KEY,
    date DATE,
    booking_id BIGINT,
    cust_id INT,
    car_id INT,
    handover_date DATE,
    return_date DATE,
    rental_amt DOUBLE,
    total_addon_amt DOUBLE,
    total_amt DOUBLE,
    customer_details VARCHAR(255),
    rate VARCHAR(255),
    fuel_amt DOUBLE,
    fuel_charge DOUBLE
);

-- INVOICE DETAIL
CREATE TABLE invoice_detail_table (
    invdtl_id BIGINT AUTO_INCREMENT PRIMARY KEY,
    invoice_id BIGINT,
    addon_id INT,
    addon_amt DOUBLE,
    FOREIGN KEY (invoice_id) REFERENCES invoice_header_table(invoice_id),
    FOREIGN KEY (addon_id) REFERENCES add_on_master(add_on_id)
);

-- SUPPORT TICKETS
CREATE TABLE support_tickets (
    id BIGINT AUTO_INCREMENT PRIMARY KEY,
    created_at DATETIME(6),
    email VARCHAR(255),
    message TEXT,
    name VARCHAR(255),
    status VARCHAR(255),
    subject VARCHAR(255)
);

-- USERS
CREATE TABLE user (
    id INT AUTO_INCREMENT PRIMARY KEY,
    username VARCHAR(255) UNIQUE,
    password VARCHAR(255) UNIQUE,
    email VARCHAR(255) UNIQUE,
    role VARCHAR(255),
    hub_id INT,
    reset_token VARCHAR(255),
    reset_token_expiry DATETIME(6),
    FOREIGN KEY (hub_id) REFERENCES hub_master(hub_id)
);

-- VENDORS
CREATE TABLE vendors (
    vendor_id BIGINT AUTO_INCREMENT PRIMARY KEY,
    api_url VARCHAR(255),
    email VARCHAR(255),
    name VARCHAR(255),
    type VARCHAR(255)
);
