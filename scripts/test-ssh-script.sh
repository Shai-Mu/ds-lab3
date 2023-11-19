username=${1:-${USERNAME}}
hostname=${2:-${HOSTNAME}}

echo "Testing ssh connection"

ssh -i ~/.ssh/id_rsa -o StrictHostKeyChecking=no $username@$hostname "ls"