# ChatOTP
ChatOTP is a chat protocol designed for anonymity and security through one-time pads. Each client generates a unique one-time key for encryption per session.

**Note:** This project was created as a part of term project for Cryptography and is not recommended for production purposes as of now.

# ChatOTP Server
This repository contains source code for ChatOTP Server. The backend uses PostgresSQL and REST API controllers for handling user data and files respectively.

## Encryption Algorithms
1. Xor Cipher
2. Rc4 Cipher

**Note:** Rc4 cipher generates a keystream which can be used for encryption by combining it with plaintext using bitwise Xor.

## Running the server
1. Change origins of CORS to corresponding domain(s) that your client is hosted on.
2. Create `data` directory and put `docker-compose.yml` outside the `data` directory. The `data` directory consists of the database.
3. Use `docker compose up` to auto start the server.
4. You can use `docker compose down` to stop the server.
