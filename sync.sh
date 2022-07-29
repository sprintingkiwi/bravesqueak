echo "Sync..."
git pull
git add .
git commit -m "sync"
git push origin master
echo "Done."
sleep 2