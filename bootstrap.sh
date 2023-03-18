#!/usr/bin/env bash

DATABASE_NAME="data.db"
DATABASE_PATH="$(pwd)/src/Database/${DATABASE_NAME}"

if [[ -f $DATABASE_PATH ]]; then
    echo "${DATABASE_NAME} already exists!"
else
    sqlite3 ./src/Database/data.db "VACUUM;"
fi
