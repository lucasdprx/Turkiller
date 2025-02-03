const express = require('express');
const mysql = require('mysql');
const bodyParser = require('body-parser');
const cors = require('cors');
const bcrypt = require('bcryptjs');
const validator = require('validator');


const app = express();
const port = 3000;

// Middleware
app.use(bodyParser.json());
app.use(cors());

// Configuration de la connexion à la base de données
const db = mysql.createConnection({
  host: 'localhost',
  user: 'root',
  password: '',
  database: 'turkill_db'
});

// Connexion à la base de données
db.connect((err) => {
  if (err) {
    throw err;
  }
  console.log('Connecté à la base de données MySQL');
});

// Route pour l'inscription
app.post('/register', async (req, res) => {
    const { username, email, password } = req.body;
  
    // Validate email format using the validator library
    if (!validator.isEmail(email)) {
      return res.status(400).send('Invalid email format');
    }
  
    // Check if the email already exists
    const emailQuery = 'SELECT * FROM users WHERE email = ?';
    db.query(emailQuery, [email], async (err, results) => {
      if (err) {
        console.error('Database error during email check:', err);
        return res.status(500).send('Error checking email');
      }
  
      // If email already exists, return an error
      if (results.length > 0) {
        return res.status(400).send('Email already in use');
      }
  
      // Check if the username already exists
      const usernameQuery = 'SELECT * FROM users WHERE username = ?';
      db.query(usernameQuery, [username], async (err, results) => {
        if (err) {
          console.error('Database error during username check:', err);
          return res.status(500).send('Error checking username');
        }
  
        // If username already exists, return an error
        if (results.length > 0) {
          return res.status(400).send('Username already taken');
        }
  
        // If email and username are unique, proceed to hash the password
        const saltRounds = 10;
        let hashedPassword;
  
        try {
          hashedPassword = await bcrypt.hash(password, saltRounds);
        } catch (error) {
          console.error('Error during bcrypt.hash:', error);
          return res.status(500).send('Error hashing password');
        }
  
        // Insert the new user into the database
        const query = 'INSERT INTO users (username, email, password) VALUES (?, ?, ?)';
        db.query(query, [username, email, hashedPassword], (err, result) => {
          if (err) {
            console.error('Database error during registration:', err);
            return res.status(500).send(err);
          }
          console.log('User successfully registered:', result);
          res.status(201).send('Utilisateur enregistré');
        });
      });
    });
  });

// Route pour la connexion
app.post('/login', async (req, res) => {
  const { email, password } = req.body;
  console.log('Login request:', req.body);

  const query = 'SELECT * FROM users WHERE email = ?';
  db.query(query, [email], async (err, results) => {
    if (err) {
      console.error('Database error during login:', err);
      return res.status(500).send(err);
    }
    
    if (results.length > 0) {
      const user = results[0];

      // Comparer le mot de passe haché
      const isPasswordValid = await bcrypt.compare(password, user.password);
      if (isPasswordValid) {
        console.log('Login successful for user:', user.username);
        res.status(200).send('Connexion réussie');
      } else {
        console.log('Incorrect password for user:', user.username);
        res.status(401).send('Email ou mot de passe incorrect');
      }
    } else {
      console.log('No user found with the given email:', email);
      res.status(401).send('Email ou mot de passe incorrect');
    }
  });
});

// Démarrer le serveur
app.listen(port, () => {
  console.log(`Serveur démarré sur http://localhost:${port}`);
});


app.get('/users', (req, res) => {
    // Query to get all users, excluding the password field
    const query = 'SELECT id, username, email FROM users';
    
    db.query(query, (err, results) => {
      if (err) {
        console.error('Database error during user fetch:', err);
        return res.status(500).send('Error retrieving users');
      }
      
      // Return the list of users (without passwords)
      res.status(200).json(results);
    });
  });
  