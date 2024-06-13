const express = require('express')
const app = express()
const port = 7999

app.use(express.static('public'))

app.listen(port, ()=> {
    console.log(`Server is running at port ${port}`)
})