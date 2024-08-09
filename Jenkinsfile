pipeline {
    agent any
    
    environment{
        UNITY_VERSION = '2022.3.22f1'
    }
    
    stages {

        stage('Init') {
            steps {
                script{
                    def user = currentBuild.getBuildCauses().shortDescription.size() > 1 ?  currentBuild.getBuildCauses().shortDescription[1] : currentBuild.getBuildCauses().shortDescription[0]
                    def msg = "${JOB_NAME} - #${BUILD_NUMBER} ${user} (<${env.BUILD_URL}|Open>)\nEnvironment: ${ENVIRONMENT}\nPlatform: ${PLATFORM}\nVersion: ${VERSION_NUMBER} (${BUILD_NUMBER})"
                    echo "Slack msg: "+msg
                    slackSend channel: '#game-builds', color: '#F5EB18', message: msg, tokenCredentialId: "slack-bot-token" 
                }
            }
        }

        stage('Version Number') {
            steps {
                script {
                    echo "Version info changing..."
                    sh '''#!/bin/bash
                                cd ${WORKSPACE}/Assets
                                echo ${VERSION_NUMBER}-${BUILD_NUMBER} > version.txt
                        '''
                    echo "Version info changed!"
                }
            }
        }

        stage('Building') {
            steps {
                script {
                    echo "Building..."
                    def unityExecutable = "/Applications/Unity/Hub/Editor/${UNITY_VERSION}/Unity.app/Contents/MacOS/Unity"
                    def buildCommand = ""
                    
                    if(PLATFORM == "iOS"){
                        if(ENVIRONMENT == "Release"){
                            buildCommand = "${unityExecutable} -nographics -buildTarget ${PLATFORM} -quit -batchmode -username ${UNITY_USERNAME} -password ${UNITY_PASSWORD} -projectPath -executeMethod BuildHelper.BuildiOSProd ${JOB_NAME}-${BUILD_NUMBER} ${WORKSPACE}/Builds/${PLATFORM}/${ENVIRONMENT}/${BUILD_NUMBER}"
                        }else if(ENVIRONMENT == "Main"){
                            buildCommand = "${unityExecutable} -nographics -buildTarget ${PLATFORM} -quit -batchmode -username ${UNITY_USERNAME} -password ${UNITY_PASSWORD} -projectPath -executeMethod BuildHelper.BuildiOSMain ${JOB_NAME}-${BUILD_NUMBER} ${WORKSPACE}/Builds/${PLATFORM}/${ENVIRONMENT}/${BUILD_NUMBER}"
                        }else {
                            buildCommand = "${unityExecutable} -nographics -buildTarget ${PLATFORM} -quit -batchmode -username ${UNITY_USERNAME} -password ${UNITY_PASSWORD} -projectPath -executeMethod BuildHelper.BuildiOSDev ${JOB_NAME}-${BUILD_NUMBER} ${WORKSPACE}/Builds/${PLATFORM}/${ENVIRONMENT}/${BUILD_NUMBER}"
                        }
                    }else {
                        sh '''#!/bin/bash
                                rm -rf ${WORKSPACE}/Builds/${PLATFORM}/${ENVIRONMENT}/${BUILD_NUMBER}/
                        '''
                        if(ENVIRONMENT == "Release"){
                            buildCommand = "${unityExecutable} -nographics -buildTarget ${PLATFORM} -quit -batchmode -username ${UNITY_USERNAME} -password ${UNITY_PASSWORD} -projectPath -executeMethod BuildHelper.BuildAndroidProd ${JOB_NAME}-${BUILD_NUMBER} ${WORKSPACE}/Builds/${PLATFORM}/${ENVIRONMENT}/${BUILD_NUMBER}"
                        }else if(ENVIRONMENT == "Main"){
                            buildCommand = "${unityExecutable} -nographics -buildTarget ${PLATFORM} -quit -batchmode -username ${UNITY_USERNAME} -password ${UNITY_PASSWORD} -projectPath -executeMethod BuildHelper.BuildAndroidMain ${JOB_NAME}-${BUILD_NUMBER} ${WORKSPACE}/Builds/${PLATFORM}/${ENVIRONMENT}/${BUILD_NUMBER}"
                        }else {
                            buildCommand = "${unityExecutable} -nographics -buildTarget ${PLATFORM} -quit -batchmode -username ${UNITY_USERNAME} -password ${UNITY_PASSWORD} -projectPath -executeMethod BuildHelper.BuildAndroidDev ${JOB_NAME}-${BUILD_NUMBER} ${WORKSPACE}/Builds/${PLATFORM}/${ENVIRONMENT}/${BUILD_NUMBER}"
                        }
                    }

                    def buildResult = sh script: buildCommand, returnStatus: true
                    
                    if (buildResult != 0) {
                        currentBuild.result = 'failure'
                        error('Build failed. See console output for details.')
                    }
                }
            }
        }

        stage('Upload Artifact')
        {
            steps{
                script{
                    if (PLATFORM == "iOS")
                    {
                        echo "Fastline starting..."
                        def buildOutputPath = "${WORKSPACE}/Builds/${PLATFORM}/${ENVIRONMENT}/${BUILD_NUMBER}"
                        echo "Path: ${buildOutputPath}"
                        sh '''#!/bin/bash
                                cd ${WORKSPACE}/Builds/${PLATFORM}/${ENVIRONMENT}/${BUILD_NUMBER}
                                fastlane update_code_signing
                                fastlane test
                        '''
                    }
                }
            }
        }
        
    }
    post {
        success {
            script 
            {
                def buildFolder = "Builds/${PLATFORM}/${ENVIRONMENT}/${BUILD_NUMBER}"

                def successMsg = "${JOB_NAME} - #${BUILD_NUMBER} Success after ${currentBuild.durationString.replace(' and counting', '')} (<${env.BUILD_URL}|Open>)";
                
                def successMsgResponse = slackSend(channel: "#game-builds", color: 'good', message: successMsg)

                if (PLATFORM == "Android")
                {
                    slackUploadFile(channel: "#game-builds:" + successMsgResponse.ts, filePath: "${buildFolder}/*")
                }
            }
        }
        failure {
            script {
                def buildStatus = currentBuild.result == 'SUCCESS' ? 'succeeded' : 'failed'
                if (currentBuild.result != 'succeeded'){
                    def failMsg = "${JOB_NAME} - #${BUILD_NUMBER} ${currentBuild.result} after ${currentBuild.durationString.replace(' and counting', '')} (<${env.BUILD_URL}|Open>)";
                    slackSend channel: "#game-builds", color: 'danger', message: failMsg
                }
            }
        }
    }
}