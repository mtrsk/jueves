#!/usr/bin/env bash

DATABASE_NAME="data.db"
DATABASE_PATH="$(pwd)/database/${DATABASE_NAME}"

if [[ -f $DATABASE_PATH ]]; then
    echo "${DATABASE_NAME} already exists!"
else
    sqlite3 $DATABASE_PATH "VACUUM;"
fi
