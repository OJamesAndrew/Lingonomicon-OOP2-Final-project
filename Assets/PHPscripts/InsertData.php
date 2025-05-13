<?php
// InsertData.php

$servername = "localhost";
$username = "root";  
$password = "";      
$dbname = "lingonomiconschema";  

$conn = new mysqli($servername, $username, $password, $dbname);

if ($conn->connect_error) {
    die("Connection failed: " . $conn->connect_error);
}

$sql = $_POST['sqlPost'];
$result = mysqli_query($conn, $sql);

$data = array();
while ($row = mysqli_fetch_assoc($result)) {
    $data[] = $row; 
}

echo json_encode($data);
?>
