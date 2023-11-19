deploy_username=${1:-${DEPLOY_USERNAME}}
deploy_hostname=${2:-${DEPLOY_ADDRESS}}

echo "Testing ssh connection"

ssh -i ~/.ssh/id_rsa -o StrictHostKeyChecking=no $deploy_username@$deploy_hostname "ls"