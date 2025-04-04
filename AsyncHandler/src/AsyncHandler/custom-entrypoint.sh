#!/bin/sh

# AWS container is launched as read-only system so I need to move the whole playwright browsers installation
cp -r /.cache/ms-playwright/ /tmp/ms-playwright/

/lambda-entrypoint.sh $@
